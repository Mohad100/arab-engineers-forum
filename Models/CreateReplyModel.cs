using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Fourm.Models;

/// <summary>
/// Model for creating a reply
/// </summary>
public class CreateReplyModel
{
    [Required(ErrorMessage = "Reply content is required")]
    [StringLength(3000, MinimumLength = 1, ErrorMessage = "Reply must be between 1 and 3000 characters")]
    public string Content { get; set; } = string.Empty;
    
    public IFormFile? Attachment { get; set; }
}
