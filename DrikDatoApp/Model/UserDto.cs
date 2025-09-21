using System.Text.Json.Serialization;

namespace DrikDatoApp.Model;

public class GetUserResponse
{
    [JsonPropertyName("status")]
    public required int Status { get; set; }
    
    [JsonPropertyName("msg")]
    public required string Message { get; set; }
    
    [JsonPropertyName("player")]
    public UserDto? Player { get; set; }
}

public class GetSelfUserResponse
{
    [JsonPropertyName("status")]
    public required int Status { get; set; }
    
    [JsonPropertyName("msg")]
    public required string Message { get; set; }
    
    [JsonPropertyName("player")]
    public SelfUserDto? Player { get; set; }
}

public class UserDto
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("alias")]
    public required string Alias { get; set; }
    
    [JsonPropertyName("phone")]
    public required string Phone { get; set; }
    
    [JsonPropertyName("token")]
    public required string? Token { get; set; }
    
    [JsonPropertyName("photo")]
    public required string? Photo { get; set; }
    
    [JsonPropertyName("glassNumber")]
    public required string GlassNumber { get; set; }
    
    [JsonPropertyName("numDates")]
    public required int NumDates { get; set; }
}

public class SelfUserDto
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("alias")]
    public required string Alias { get; set; }
    
    [JsonPropertyName("phone")]
    public required string Phone { get; set; }
    
    [JsonPropertyName("token")]
    public required string? Token { get; set; }
    
    [JsonPropertyName("photo")]
    public required string? Photo { get; set; }
    
    [JsonPropertyName("glassNumber")]
    public required string GlassNumber { get; set; }
    
    [JsonPropertyName("dates")]
    public required List<DateEntry> Dates { get; set; }
    
    [JsonPropertyName("achievements")]
    public required List<Achievement> Achievements { get; set; }
}

public class DateEntry
{
    [JsonPropertyName("dateid")]
    public required int DateId { get; set; }
    
    [JsonPropertyName("comment")]
    public required string Comment { get; set; }
    
    [JsonPropertyName("status")]
    public required string Status { get; set; }
    
    [JsonPropertyName("enddate")]
    public required string EndDate { get; set; }
    
    [JsonPropertyName("date_formatted")]
    public required string DateFormatted { get; set; }
    
    [JsonPropertyName("enddate_formatted")]
    public required string EndDateFormatted { get; set; }
}

public class Achievement
{
    [JsonPropertyName("achievementSlug")]
    public required string AchievementSlug { get; set; }
    
    [JsonPropertyName("numDates")]
    public required string NumDates { get; set; }
    
    [JsonPropertyName("created")]
    public required string Created { get; set; }
    
    [JsonPropertyName("engraved")]
    public required string Engraved { get; set; }
}
