using System.ComponentModel.DataAnnotations;

namespace Fourm.Models;

/// <summary>
/// Model for the fun entertainment form
/// </summary>
public class FunFormModel
{
    [Required(ErrorMessage = "We need to know what to call you!")]
    [StringLength(100, ErrorMessage = "That's a bit too long for a name, don't you think?")]
    public string Nickname { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Keep it under 200 characters, please!")]
    public string? FavoriteEntertainment { get; set; }

    [Required(ErrorMessage = "Come on, tell us how you're feeling!")]
    public string MoodToday { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "That's quite a fun fact! Try keeping it under 500 characters.")]
    public string? FunFact { get; set; }

    [Required(ErrorMessage = "You have to pick one!")]
    public string WouldYouRather { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; } = DateTime.Now;
    public string Username { get; set; } = string.Empty;
}
