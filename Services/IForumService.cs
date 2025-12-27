using Fourm.Models;

namespace Fourm.Services;

/// <summary>
/// Interface for forum thread and reply management
/// </summary>
public interface IForumService
{
    // Thread operations
    Task<ForumThread> CreateThreadAsync(string categoryId, string title, string content, string username, string? imageUrl = null);
    Task<ForumThread?> GetThreadByIdAsync(Guid threadId);
    Task<List<ForumThread>> GetAllThreadsAsync();
    Task<List<ForumThread>> GetThreadsByCategoryAsync(string categoryId);
    Task<bool> DeleteThreadAsync(Guid threadId, string username);
    Task<bool> UpdateThreadAsync(Guid threadId, string title, string content, string username, string? imageUrl = null);
    
    // Reply operations
    Task<ThreadReply> CreateReplyAsync(Guid threadId, string content, string username, string? attachmentUrl = null, string? attachmentFileName = null);
    Task<List<ThreadReply>> GetThreadRepliesAsync(Guid threadId);
    Task<bool> DeleteReplyAsync(Guid replyId, string username);
    Task<bool> UpdateReplyAsync(Guid replyId, string content, string username);

    // Admin operations
    Task<bool> MarkThreadAsViolationAsync(Guid threadId, string violationReason, string adminUsername);
    Task<bool> MarkReplyAsViolationAsync(Guid replyId, string violationReason, string adminUsername);
    Task<bool> RemoveViolationFromThreadAsync(Guid threadId);
    Task<bool> RemoveViolationFromReplyAsync(Guid replyId);
    Task<bool> AdminDeleteThreadAsync(Guid threadId);
    Task<bool> AdminDeleteReplyAsync(Guid replyId);
    Task<List<ForumThread>> GetAllThreadsWithViolationsAsync();
}
