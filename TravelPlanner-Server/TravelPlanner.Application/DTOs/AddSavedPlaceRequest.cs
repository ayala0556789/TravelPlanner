namespace TravelPlanner.Application.DTOs;

public record AddSavedPlaceRequest(string PlaceName, string GooglePlaceId, string? Address);