using MediatR;
using TravelPlanner.Domain.Entities;
using TravelPlanner.Domain.Interfaces;

namespace TravelPlanner.Application.Features.SavedPlaces.Commands.AddSavedPlace;

public class AddSavedPlaceHandler : IRequestHandler<AddSavedPlaceCommand, int>
{
    private readonly ISavedPlaceRepository _repository;

    public AddSavedPlaceHandler(ISavedPlaceRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(AddSavedPlaceCommand request, CancellationToken cancellationToken)
    {
        // יצירת אובייקט חדש מסוג SavedPlace לפי הנתונים שהגיעו מהפקודה
        var newPlace = new SavedPlace
        {
            UserId = request.UserId,
            PlaceName = request.PlaceName,
            GooglePlaceId = request.GooglePlaceId,
            Address = request.Address,
            SavedAt = DateTime.UtcNow // זמן השמירה הנוכחי
        };

        // שימוש ב-Repository שכתבת כדי להוסיף למסד הנתונים
        await _repository.AddSavedPlaceAsync(newPlace);

        // שמירת השינויים הסופית (הפונקציה שקיימת ב-Repository שלך)
        await _repository.SaveChangesAsync();

        // החזרת ה-ID שנוצר אוטומטית ע"י מסד הנתונים
        return newPlace.Id;
    }
}