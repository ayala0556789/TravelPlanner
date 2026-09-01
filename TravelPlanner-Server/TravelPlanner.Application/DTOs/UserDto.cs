namespace TravelPlanner.Application.DTOs;

public class UserDto
{
    public int Id { get; set; } 
    public string IdentityNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string HomeAddress { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Token { get; set; }
    public List<int> ChildrenAges { get; set; } = new();     // שונה מ-string ל-List<int>
    public bool PreferAccessiblePaths { get; set; }
}