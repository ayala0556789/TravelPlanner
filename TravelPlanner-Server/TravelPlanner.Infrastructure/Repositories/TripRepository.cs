using Microsoft.EntityFrameworkCore;
using TravelPlanner.Domain.Entities;
using TravelPlanner.Domain.Interfaces;
using TravelPlanner.Infrastructure.Persistence;

namespace TravelPlanner.Infrastructure.Repositories;

public class TripRepository : ITripRepository
{
    private readonly ApplicationDbContext _context;

    public TripRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // שליפת טיול כולל רשימת המקומות שלו (Include) ממוינים לפי הסדר
    public async Task<Trip?> GetTripWithPlacesAsync(int tripId, int userId)
    {
        return await _context.Trips
            .Include(t => t.TripPlaces.OrderBy(p => p.OrderIndex))
            .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);
    }

    // שליפת כל הטיולים של משתמש ספציפי להצגה בהיסטוריה
    public async Task<IEnumerable<Trip>> GetUserTripsAsync(int userId)
    {
        return await _context.Trips
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Id) // הטיולים החדשים יופיעו ראשונים
            .ToListAsync();
    }

    // שמירת טיול חדש
    public async Task AddTripAsync(Trip trip)
    {
        await _context.Trips.AddAsync(trip);
    }

    // עדכון סדר הנקודות (Drag & Drop)
    public async Task UpdateTripPlacesOrderAsync(int tripId, List<TripPlace> updatedPlaces)
    {
        // אנחנו מעדכנים רק את ה-OrderIndex לכל מקום ברשימה
        foreach (var place in updatedPlaces)
        {
            _context.Entry(place).Property(p => p.OrderIndex).IsModified = true;
        }
    }

    // עדכון רשימת הציוד (שמירת ה-JSON שה-AI ייצר)
    public async Task UpdateEquipmentListAsync(int tripId, int userId, string equipmentJson)
    {
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);
        if (trip != null)
        {
            trip.EquipmentList = equipmentJson;
        }
    }

    // שליפת ה-JSON של הציוד בלבד עבור האנגולר
    public async Task<string> GetEquipmentListAsync(int tripId, int userId)
    {
        // אנחנו מחפשים טיול שמתאים גם ל-ID של הטיול וגם ל-ID של המשתמש
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);

        // אם לא נמצא טיול כזה (או שהוא שייך למישהו אחר), נחזיר מחרוזת ריקה
        return trip?.EquipmentList ?? string.Empty;
    }

    // מחיקת טיול מהמערכת
    public async Task DeleteTripAsync(int tripId, int userId)
    {
        var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);
        if (trip != null)
        {
            _context.Trips.Remove(trip);
        }
    }

    // ביצוע השמירה הסופית ל-DB (Commit)
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}




