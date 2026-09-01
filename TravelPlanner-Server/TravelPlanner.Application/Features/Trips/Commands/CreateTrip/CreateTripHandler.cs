using MediatR;
using TravelPlanner.Domain.DTOs;
using TravelPlanner.Domain.Entities;
using TravelPlanner.Domain.Interfaces;

namespace TravelPlanner.Application.Features.Trips.Commands.CreateTrip;

public class CreateTripCommandHandler : IRequestHandler<CreateTripCommand, int>
{
    private readonly ITripRepository _tripRepository;
    private readonly IMapsService _mapsService;
    private readonly IAIService _aiService;
    private readonly IUserRepository _userRepository;
    private readonly IWeatherService _weatherService;

    // הזרקת ה-Repository - 
    public CreateTripCommandHandler(ITripRepository tripRepository, IMapsService mapsService,IAIService aiService,IUserRepository userRepository, IWeatherService weatherService)
    {
        _tripRepository = tripRepository;
        _mapsService = mapsService;
        _aiService = aiService;
        _userRepository = userRepository;
        _weatherService = weatherService;
    }

    public async Task<int> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    {
        //1. פנייה לגוגל כדי להביא את האטרקציות/מקומות באופן אוטומטי
        var places = await _mapsService.GetPlacesForTripAsync(request.Destination, request.Category);

        // : שליפת מזג אוויר לפי קואורדינטות המקום הראשון (מייצג את היעד)
        (double Temp, string Condition) weather = (0, "Unknown");
        if (places.Count > 0)
        {
            weather = await _weatherService.GetWeatherAsync(
                places[0].Latitude, places[0].Longitude);
        }

        // 2. שליפת העדפות המשתמש - כדי שה-AI ידע "מי המטייל"
        var userPreferences = await _userRepository.GetUserTravelContextAsync(request.UserId);

        var effectivePrefs = new UserPreferencesDto
        {
            HomeAddress = userPreferences.HomeAddress,
            PreferAccessiblePaths = userPreferences.PreferAccessiblePaths,
            ChildrenAges = request.SelectedChildrenAges
        };

        // 3. שליחה ל-AI: בחירה, סידור גיאוגרפי, והסבר לכל מקום (RAG!)
        var finalPlaces = await _aiService.SelectAndEnrichPlacesAsync(
            places,
            effectivePrefs,
            request.DurationInHours,
            request.Category,
            request.TravelDate,
            request.Budget,
            weather.Condition
            );

        //4. יצירת אובייקט Trip חדש על בסיס הנתונים מה-Command
        // יצירת אובייקט הטיול - עכשיו עם המקומות שה-AI בחר וסידר

        var newTrip = new Trip
        {
            Destination = request.Destination,  // יעד הטיול
            TravelDate = request.TravelDate,     // תאריך הטיול (ב-Entity זה TravelDate)
            UserId = request.UserId,            // קישור למשתמש שיצר את הטיול
            EquipmentList = "{}",               // אתחול רשימת ציוד ריקה במבנה JSON כפי שהגדרת ב-Trip
            DurationInHours = request.DurationInHours,            // ערך ברירת מחדל (ניתן להוסיף ל-Command אם תרצה)
            TripPlaces = finalPlaces,
            PredictedTemperature = weather.Temp,     
            WeatherCondition = weather.Condition 
            //TripPlaces = places ?? new List<TripPlace>()
        };

        // 2. שמירה בבסיס הנתונים
        // שים לב: קראנו ל-AddTripAsync כי זה השם שהגדרת ב-ITripRepository
        await _tripRepository.AddTripAsync(newTrip);

        // ביצוע השמירה הסופית (Commit) ל-DB
        await _tripRepository.SaveChangesAsync();

        // 3. החזרת ה-ID שנוצר באופן אוטומטי במסד הנתונים
        return newTrip.Id;
    }
}