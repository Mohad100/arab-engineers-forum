using System.ComponentModel.DataAnnotations;

namespace Fourm.Models;

/// <summary>
/// Represents a reply to a forum thread
/// </summary>
public class ThreadReply
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid ThreadId { get; set; }
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public string AuthorUsername { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public bool IsEdited { get; set; } = false;
    
    public DateTime? EditedAt { get; set; }

    // Attachment support
    public string? AttachmentUrl { get; set; }
    public string? AttachmentFileName { get; set; }

    // Violation tracking
    public bool IsViolation { get; set; } = false;
    public string? ViolationReason { get; set; }
    public DateTime? ViolationDate { get; set; }
    public string? ViolatedByAdmin { get; set; }
}
