using System.Text.Json.Serialization;

namespace TravelPlanner.Infrastructure.ExternalServices.AI;

// --- מבנה הבקשה שנשלחת ל-Gemini ---
public class GeminiRequest
{
    [JsonPropertyName("contents")]
    public List<GeminiContent> Contents { get; set; } = new();

    // חדש: כופה על Gemini להחזיר JSON תקני ברמת ה-Protocol, לא רק בבקשה בטקסט
    [JsonPropertyName("generationConfig")]
    public GeminiGenerationConfig GenerationConfig { get; set; } = new();
}

// : מחלקת ההגדרות
public class GeminiGenerationConfig
{
    [JsonPropertyName("response_mime_type")]
    public string ResponseMimeType { get; set; } = "application/json";
}
public class GeminiContent
{
    [JsonPropertyName("parts")]
    public List<GeminiPart> Parts { get; set; } = new();
}

public class GeminiPart
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

// --- מבנה התשובה מ-Gemini ---
public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<GeminiCandidate> Candidates { get; set; } = new();
}

public class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContent Content { get; set; } = new();
}

// --- המבנה שאנחנו מבקשים מהמודל להחזיר בתוך הטקסט (JSON פנימי) ---
public class AiSelectionResult
{
    [JsonPropertyName("selectedPlaces")]
    public List<AiSelectedPlace> SelectedPlaces { get; set; } = new();
}

public class AiSelectedPlace
{
    [JsonPropertyName("googlePlaceId")]
    public string GooglePlaceId { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}