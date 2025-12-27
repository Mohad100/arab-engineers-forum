using System.ComponentModel.DataAnnotations;

namespace Fourm.Models;

/// <summary>
/// Model for creating a new thread
/// </summary>
public class CreateThreadModel
{
    [Required(ErrorMessage = "Please select a category")]
    public string CategoryId { get; set; } = "general";
    
    [Required(ErrorMessage = "Thread title is required")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Thread content is required")]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 5000 characters")]
    public string Content { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }
}
