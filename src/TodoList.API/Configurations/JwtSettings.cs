using System.ComponentModel.DataAnnotations;

namespace TodoList.API.Configurations;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    [Required(ErrorMessage = "Key is required.")]
    [MinLength(16, ErrorMessage = "Key must be at least 16 characters long.")]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int ExpireMinutes { get; set; } = 60;
}