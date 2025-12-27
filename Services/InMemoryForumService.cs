using Fourm.Models;

namespace Fourm.Services;

/// <summary>
/// In-memory storage for forum threads and replies
/// </summary>
public class InMemoryForumService : IForumService
{
    private readonly List<ForumThread> _threads = new();
    private readonly List<ThreadReply> _replies = new();
    private readonly object _lock = new();

    public Task<ForumThread> CreateThreadAsync(string categoryId, string title, string content, string username, string? imageUrl = null)
    {
        lock (_lock)
        {
            var thread = new ForumThread
            {
                CategoryId = categoryId,
                Title = title,
                Content = content,
                AuthorUsername = username,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.Now,
                LastReplyAt = DateTime.Now
            };

            _threads.Add(thread);
            return Task.FromResult(thread);
        }
    }

    public Task<ForumThread?> GetThreadByIdAsync(Guid threadId)
    {
        lock (_lock)
        {
            var thread = _threads.FirstOrDefault(t => t.Id == threadId);
            return Task.FromResult(thread);
        }
    }

    public Task<List<ForumThread>> GetAllThreadsAsync()
    {
        lock (_lock)
        {
            // Return threads ordered by pinned first, then by last reply time
            return Task.FromResult(_threads
                .OrderByDescending(t => t.IsPinned)
                .ThenByDescending(t => t.LastReplyAt)
                .ToList());
        }
    }

    public Task<List<ForumThread>> GetThreadsByCategoryAsync(string categoryId)
    {
        lock (_lock)
        {
            return Task.FromResult(_threads
                .Where(t => t.CategoryId.Equals(categoryId, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(t => t.IsPinned)
                .ThenByDescending(t => t.LastReplyAt)
                .ToList());
        }
    }

    public Task<bool> DeleteThreadAsync(Guid threadId, string username)
    {
        lock (_lock)
        {
            var thread = _threads.FirstOrDefault(t => t.Id == threadId);
            if (thread != null && thread.AuthorUsername == username)
            {
                _threads.Remove(thread);
                // Also remove all replies to this thread
                _replies.RemoveAll(r => r.ThreadId == threadId);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<bool> UpdateThreadAsync(Guid threadId, string title, string content, string username, string? imageUrl = null)
    {
        lock (_lock)
        {
            var thread = _threads.FirstOrDefault(t => t.Id == threadId);
            if (thread != null && thread.AuthorUsername == username)
            {
                thread.Title = title;
                thread.Content = content;
                if (imageUrl != null)
                {
                    thread.ImageUrl = imageUrl;
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<ThreadReply> CreateReplyAsync(Guid threadId, string content, string username, string? attachmentUrl = null, string? attachmentFileName = null)
    {
        lock (_lock)
        {
            var reply = new ThreadReply
            {
                ThreadId = threadId,
                Content = content,
                AuthorUsername = username,
                CreatedAt = DateTime.Now,
                AttachmentUrl = attachmentUrl,
                AttachmentFileName = attachmentFileName
            };

            _replies.Add(reply);

            // Update thread's last reply time and reply count
            var thread = _threads.FirstOrDefault(t => t.Id == threadId);
            if (thread != null)
            {
                thread.LastReplyAt = DateTime.Now;
                thread.ReplyCount = _replies.Count(r => r.ThreadId == threadId);
            }

            return Task.FromResult(reply);
        }
    }

    public Task<List<ThreadReply>> GetThreadRepliesAsync(Guid threadId)
    {
        lock (_lock)
        {
            return Task.FromResult(_replies
                .Where(r => r.ThreadId == threadId)
                .OrderBy(r => r.CreatedAt)
                .ToList());
        }
    }

    public Task<bool> DeleteReplyAsync(Guid replyId, string username)
    {
        lock (_lock)
        {
            var reply = _replies.FirstOrDefault(r => r.Id == replyId);
            if (reply != null && reply.AuthorUsername == username)
            {
                var threadId = reply.ThreadId;
                _replies.Remove(reply);

                // Update thread's reply count
                var thread = _threads.FirstOrDefault(t => t.Id == threadId);
                if (thread != null)
                {
                    thread.ReplyCount = _replies.Count(r => r.ThreadId == threadId);
                }

                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<bool> UpdateReplyAsync(Guid replyId, string content, string username)
    {
        lock (_lock)
        {
            var reply = _replies.FirstOrDefault(r => r.Id == replyId);
            if (reply != null && reply.AuthorUsername == username)
            {
                reply.Content = content;
                reply.IsEdited = true;
                reply.EditedAt = DateTime.Now;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    // Admin operations
    public Task<bool> MarkThreadAsViolationAsync(Guid threadId, string violationReason, string adminUsername)
    {
        lock (_lock)
        {
            var thread = _threads.FirstOrDefault(t => t.Id == threadId);
            if (thread != null)
            {
                thread.IsViolation = true;
                thread.ViolationReason = violationReason;
                thread.ViolationDate = DateTime.Now;
                thread.ViolatedByAdmin = adminUsername;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<bool> MarkReplyAsViolationAsync(Guid replyId, string violationReason, string adminUsername)
    {
        lock (_lock)
        {
            var reply = _replies.FirstOrDefault(r => r.Id == replyId);
            if (reply != null)
            {
                reply.IsViolation = true;
                reply.ViolationReason = violationReason;
                reply.ViolationDate = DateTime.Now;
                reply.ViolatedByAdmin = adminUsername;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<bool> RemoveViolationFromThreadAsync(Guid threadId)
    {
        lock (_lock)
        {
            var thread = _threads.FirstOrDefault(t => t.Id == threadId);
            if (thread != null)
            {
                thread.IsViolation = false;
                thread.ViolationReason = null;
                thread.ViolationDate = null;
                thread.ViolatedByAdmin = null;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<bool> RemoveViolationFromReplyAsync(Guid replyId)
    {
        lock (_lock)
        {
            var reply = _replies.FirstOrDefault(r => r.Id == replyId);
            if (reply != null)
            {
                reply.IsViolation = false;
                reply.ViolationReason = null;
                reply.ViolationDate = null;
                reply.ViolatedByAdmin = null;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<bool> AdminDeleteThreadAsync(Guid threadId)
    {
        lock (_lock)
        {
            var thread = _threads.FirstOrDefault(t => t.Id == threadId);
            if (thread != null)
            {
                _threads.Remove(thread);
                _replies.RemoveAll(r => r.ThreadId == threadId);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<bool> AdminDeleteReplyAsync(Guid replyId)
    {
        lock (_lock)
        {
            var reply = _replies.FirstOrDefault(r => r.Id == replyId);
            if (reply != null)
            {
                _replies.Remove(reply);
                var thread = _threads.FirstOrDefault(t => t.Id == reply.ThreadId);
                if (thread != null)
                {
                    thread.ReplyCount = _replies.Count(r => r.ThreadId == reply.ThreadId);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<List<ForumThread>> GetAllThreadsWithViolationsAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_threads.OrderByDescending(t => t.CreatedAt).ToList());
        }
    }
}
