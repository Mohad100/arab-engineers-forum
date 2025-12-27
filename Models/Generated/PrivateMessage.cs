using System;
using System.Collections.Generic;

namespace Fourm.Models.Generated;

public partial class PrivateMessage
{
    public string Id { get; set; } = null!;

    public string SenderId { get; set; } = null!;

    public string SenderUsername { get; set; } = null!;

    public string RecipientId { get; set; } = null!;

    public string RecipientUsername { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public bool IsDeletedBySender { get; set; }

    public bool IsDeletedByRecipient { get; set; }
}
