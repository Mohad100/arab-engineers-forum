using System;
using System.Collections.Generic;

namespace Fourm.Models.Generated;

public partial class Reply
{
    public Guid Id { get; set; }

    public Guid ThreadId { get; set; }

    public string Content { get; set; } = null!;

    public string AuthorUsername { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsEdited { get; set; }

    public DateTime? EditedAt { get; set; }

    public bool IsViolation { get; set; }

    public string? ViolatedByAdmin { get; set; }

    public DateTime? ViolationDate { get; set; }

    public string? ViolationReason { get; set; }

    public virtual Thread Thread { get; set; } = null!;
}
