using System;
using System.Collections.Generic;

namespace Fourm.Models.Generated;

public partial class Thread
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string CategoryId { get; set; } = null!;

    public string AuthorUsername { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime LastReplyAt { get; set; }

    public int ReplyCount { get; set; }

    public bool IsPinned { get; set; }

    public bool IsLocked { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsViolation { get; set; }

    public string? ViolatedByAdmin { get; set; }

    public DateTime? ViolationDate { get; set; }

    public string? ViolationReason { get; set; }

    public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();
}
