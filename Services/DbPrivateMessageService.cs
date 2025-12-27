using Fourm.Data;
using Fourm.Models;
using Microsoft.EntityFrameworkCore;

namespace Fourm.Services;

public class DbPrivateMessageService : IPrivateMessageService
{
    private readonly ForumDbContext _context;

    public DbPrivateMessageService(ForumDbContext context)
    {
        _context = context;
    }

    public async Task<List<PrivateMessage>> GetInboxAsync(string username)
    {
        return await _context.PrivateMessages
            .Where(m => m.RecipientUsername.ToLower() == username.ToLower() && !m.IsDeletedByRecipient)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();
    }

    public async Task<List<PrivateMessage>> GetSentAsync(string username)
    {
        return await _context.PrivateMessages
            .Where(m => m.SenderUsername.ToLower() == username.ToLower() && !m.IsDeletedBySender)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();
    }

    public async Task<PrivateMessage?> GetMessageAsync(string messageId, string username)
    {
        var usernameLower = username.ToLower();
        return await _context.PrivateMessages
            .FirstOrDefaultAsync(m => m.Id == messageId &&
                (m.RecipientUsername.ToLower() == usernameLower || m.SenderUsername.ToLower() == usernameLower) &&
                !((m.SenderUsername.ToLower() == usernameLower && m.IsDeletedBySender) ||
                  (m.RecipientUsername.ToLower() == usernameLower && m.IsDeletedByRecipient)));
    }

    public async Task<int> GetUnreadCountAsync(string username)
    {
        return await _context.PrivateMessages
            .Where(m => m.RecipientUsername.ToLower() == username.ToLower() && !m.IsRead && !m.IsDeletedByRecipient)
            .CountAsync();
    }

    public async Task SendAsync(string senderUsername, string recipientUsername, string subject, string content)
    {
        var message = new PrivateMessage
        {
            Id = Guid.NewGuid().ToString(),
            SenderUsername = senderUsername,
            RecipientUsername = recipientUsername,
            Subject = subject,
            Content = content,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };
        
        _context.PrivateMessages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task MarkAsReadAsync(string messageId)
    {
        var message = await _context.PrivateMessages.FindAsync(messageId);
        if (message != null && !message.IsRead)
        {
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(string messageId, string username)
    {
        var message = await _context.PrivateMessages.FindAsync(messageId);
        if (message != null)
        {
            var usernameLower = username.ToLower();
            if (message.SenderUsername.ToLower() == usernameLower)
                message.IsDeletedBySender = true;
            else if (message.RecipientUsername.ToLower() == usernameLower)
                message.IsDeletedByRecipient = true;

            // If both have deleted, remove from database
            if (message.IsDeletedBySender && message.IsDeletedByRecipient)
            {
                _context.PrivateMessages.Remove(message);
            }

            await _context.SaveChangesAsync();
        }
    }
}
