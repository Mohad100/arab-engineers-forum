namespace Fourm.Models
{
    public class PrivateMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SenderId { get; set; } = string.Empty;
        public string SenderUsername { get; set; } = string.Empty;
        public string RecipientId { get; set; } = string.Empty;
        public string RecipientUsername { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public bool IsDeletedBySender { get; set; } = false;
        public bool IsDeletedByRecipient { get; set; } = false;
    }
}
