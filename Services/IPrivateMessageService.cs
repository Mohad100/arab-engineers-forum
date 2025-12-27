using Fourm.Models;

namespace Fourm.Services;

public interface IPrivateMessageService
{
    Task<List<PrivateMessage>> GetInboxAsync(string username);
    Task<List<PrivateMessage>> GetSentAsync(string username);
    Task<PrivateMessage?> GetMessageAsync(string messageId, string username);
    Task<int> GetUnreadCountAsync(string username);
    Task SendAsync(string senderUsername, string recipientUsername, string subject, string content);
    Task MarkAsReadAsync(string messageId);
    Task DeleteAsync(string messageId, string username);
}
