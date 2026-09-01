using Microsoft.EntityFrameworkCore;
using TravelPlanner.Domain.Entities;
using TravelPlanner.Domain.Interfaces;
using TravelPlanner.Infrastructure.Persistence;


namespace TravelPlanner.Infrastructure.Repositories;

public class SavedPlaceRepository : ISavedPlaceRepository
{
    private readonly ApplicationDbContext _context;

    public SavedPlaceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // שליפת כל המקומות שהמשתמש שמר כ"מועדפים"
    public async Task<IEnumerable<SavedPlace>> GetSavedPlacesByUserIdAsync(int userId)
    {
        return await _context.SavedPlaces
            .Where(sp => sp.UserId == userId)
            .ToListAsync();
    }

    // הוספת מקום חדש לרשימת השמורים
    public async Task AddSavedPlaceAsync(SavedPlace place)
    {
        await _context.SavedPlaces.AddAsync(place);
    }

    // מחיקת מקום מהמועדפים (למשל אם המשתמש התחרט)
    public async Task DeleteSavedPlaceAsync(int placeId, int userId)
    {
        var place = await _context.SavedPlaces
        .FirstOrDefaultAsync(sp => sp.Id == placeId && sp.UserId == userId);
        if (place != null)
        {
            _context.SavedPlaces.Remove(place);
        }
    }

    // ביצוע השמירה הסופית במסד הנתונים
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}