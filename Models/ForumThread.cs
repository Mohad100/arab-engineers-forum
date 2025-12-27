using System.ComponentModel.DataAnnotations;

namespace Fourm.Models;

/// <summary>
/// Represents a forum thread
/// </summary>
public class ForumThread
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [Required]
    public string CategoryId { get; set; } = "general";
    
    public string AuthorUsername { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime LastReplyAt { get; set; } = DateTime.Now;
    
    public int ReplyCount { get; set; } = 0;
    
    public bool IsPinned { get; set; } = false;
    
    public bool IsLocked { get; set; } = false;

    // Image attachment
    public string? ImageUrl { get; set; }

    // Violation tracking
    public bool IsViolation { get; set; } = false;
    public string? ViolationReason { get; set; }
    public DateTime? ViolationDate { get; set; }
    public string? ViolatedByAdmin { get; set; }

    // Navigation property
    public List<ThreadReply> Replies { get; set; } = new List<ThreadReply>();
}
