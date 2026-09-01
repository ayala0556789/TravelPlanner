using Microsoft.EntityFrameworkCore;
using TravelPlanner.Domain.DTOs;
using TravelPlanner.Domain.Entities;
using TravelPlanner.Domain.Interfaces;
using TravelPlanner.Infrastructure.Persistence;
using System.Text.Json;

namespace TravelPlanner.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    // Constructor: מקבל את ה-Context שהגדרנו כדי שנוכל לגשת לטבלאות
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // שליפת משתמש לפי ID
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    // שליפת משתמש לפי תעודת זהות (עבור התחברות)
    public async Task<User?> GetByIdentityNumberAsync(string identityNumber)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.IdentityNumber == identityNumber);
    }

    // המימוש של ה-DTO: שולף רק את מה שצריך עבור ה-AI
    public async Task<UserPreferencesDto> GetUserTravelContextAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return new UserPreferencesDto();

        return new UserPreferencesDto
        {
            HomeAddress = user.HomeAddress,
            ChildrenAges = string.IsNullOrEmpty(user.ChildrenAges)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(user.ChildrenAges) ?? new List<int>(),
            PreferAccessiblePaths = user.PreferAccessiblePaths
        };
    }
    //public async Task<UserPreferencesDto> GetUserTravelContextAsync(int userId)
    //{
    //    var user = await _context.Users
    //        .Where(u => u.Id == userId)
    //        .Select(u => new UserPreferencesDto
    //        {
    //            HasBaby = u.HasBaby,
    //            HomeAddress = u.HomeAddress
    //        })
    //        .FirstOrDefaultAsync();

    //    return user ?? new UserPreferencesDto();
    //}

    // הוספת משתמש חדש
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    // עדכון פרטים קיימים
    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    // בדיקת סיסמה (בשלב זה השוואה פשוטה, בהמשך נוסיף הצפנה)
    public async Task<bool> VerifyPasswordAsync(int userId, string password)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        return user.PasswordHash == password; // כאן תבוא לוגיקת ה-Hash בעתיד
    }
}