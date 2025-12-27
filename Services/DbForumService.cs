using Fourm.Data;
using Fourm.Models;
using Microsoft.EntityFrameworkCore;

namespace Fourm.Services;

public class DbForumService : IForumService
{
    private readonly ForumDbContext _context;

    public DbForumService(ForumDbContext context)
    {
        _context = context;
    }

    public async Task<ForumThread> CreateThreadAsync(string categoryId, string title, string content, string username, string? imageUrl = null)
    {
        var thread = new ForumThread
        {
            Id = Guid.NewGuid(),
            Title = title,
            Content = content,
            AuthorUsername = username,
            CategoryId = categoryId,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow,
            LastReplyAt = DateTime.UtcNow,
            Replies = new List<ThreadReply>()
        };

        _context.Threads.Add(thread);
        await _context.SaveChangesAsync();
        return thread;
    }

    public async Task<List<ForumThread>> GetAllThreadsAsync()
    {
        return await _context.Threads
            .Include(t => t.Replies)
            .OrderByDescending(t => t.IsPinned)
            .ThenByDescending(t => t.LastReplyAt)
            .ToListAsync();
    }

    public async Task<List<ForumThread>> GetThreadsByCategoryAsync(string categoryId)
    {
        return await _context.Threads
            .Include(t => t.Replies)
            .Where(t => t.CategoryId == categoryId)
            .OrderByDescending(t => t.IsPinned)
            .ThenByDescending(t => t.LastReplyAt)
            .ToListAsync();
    }

    public async Task<ForumThread?> GetThreadByIdAsync(Guid threadId)
    {
        return await _context.Threads
            .Include(t => t.Replies)
            .FirstOrDefaultAsync(t => t.Id == threadId);
    }

    public async Task<bool> DeleteThreadAsync(Guid threadId, string username)
    {
        var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == threadId);
        if (thread == null || thread.AuthorUsername != username)
        {
            return false;
        }

        _context.Threads.Remove(thread);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateThreadAsync(Guid threadId, string title, string content, string username, string? imageUrl = null)
    {
        var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == threadId);
        if (thread == null || thread.AuthorUsername != username)
        {
            return false;
        }

        thread.Title = title;
        thread.Content = content;
        if (imageUrl != null)
        {
            thread.ImageUrl = imageUrl;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ThreadReply> CreateReplyAsync(Guid threadId, string content, string username, string? attachmentUrl = null, string? attachmentFileName = null)
    {
        var thread = await _context.Threads
            .FirstOrDefaultAsync(t => t.Id == threadId);

        if (thread == null)
        {
            throw new InvalidOperationException("Thread not found");
        }

        var reply = new ThreadReply
        {
            Id = Guid.NewGuid(),
            ThreadId = threadId,
            Content = content,
            AuthorUsername = username,
            CreatedAt = DateTime.UtcNow,
            AttachmentUrl = attachmentUrl,
            AttachmentFileName = attachmentFileName
        };

        _context.Replies.Add(reply);
        thread.LastReplyAt = DateTime.UtcNow;
        thread.ReplyCount++;

        await _context.SaveChangesAsync();
        return reply;
    }

    public async Task<List<ThreadReply>> GetThreadRepliesAsync(Guid threadId)
    {
        var thread = await _context.Threads
            .Include(t => t.Replies)
            .FirstOrDefaultAsync(t => t.Id == threadId);

        return thread?.Replies.OrderBy(r => r.CreatedAt).ToList() ?? new List<ThreadReply>();
    }

    public async Task<bool> DeleteReplyAsync(Guid replyId, string username)
    {
        var reply = await _context.Replies.FirstOrDefaultAsync(r => r.Id == replyId);
        if (reply == null || reply.AuthorUsername != username)
        {
            return false;
        }

        _context.Replies.Remove(reply);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateReplyAsync(Guid replyId, string content, string username)
    {
        var reply = await _context.Replies.FirstOrDefaultAsync(r => r.Id == replyId);
        if (reply == null || reply.AuthorUsername != username)
        {
            return false;
        }

        reply.Content = content;
        reply.IsEdited = true;
        reply.EditedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // Admin operations
    public async Task<bool> MarkThreadAsViolationAsync(Guid threadId, string violationReason, string adminUsername)
    {
        var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == threadId);
        if (thread == null)
        {
            return false;
        }

        thread.IsViolation = true;
        thread.ViolationReason = violationReason;
        thread.ViolationDate = DateTime.UtcNow;
        thread.ViolatedByAdmin = adminUsername;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkReplyAsViolationAsync(Guid replyId, string violationReason, string adminUsername)
    {
        var reply = await _context.Replies.FirstOrDefaultAsync(r => r.Id == replyId);
        if (reply == null)
        {
            return false;
        }

        reply.IsViolation = true;
        reply.ViolationReason = violationReason;
        reply.ViolationDate = DateTime.UtcNow;
        reply.ViolatedByAdmin = adminUsername;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveViolationFromThreadAsync(Guid threadId)
    {
        var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == threadId);
        if (thread == null)
        {
            return false;
        }

        thread.IsViolation = false;
        thread.ViolationReason = null;
        thread.ViolationDate = null;
        thread.ViolatedByAdmin = null;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveViolationFromReplyAsync(Guid replyId)
    {
        var reply = await _context.Replies.FirstOrDefaultAsync(r => r.Id == replyId);
        if (reply == null)
        {
            return false;
        }

        reply.IsViolation = false;
        reply.ViolationReason = null;
        reply.ViolationDate = null;
        reply.ViolatedByAdmin = null;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AdminDeleteThreadAsync(Guid threadId)
    {
        var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == threadId);
        if (thread == null)
        {
            return false;
        }

        _context.Threads.Remove(thread);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AdminDeleteReplyAsync(Guid replyId)
    {
        var reply = await _context.Replies.FirstOrDefaultAsync(r => r.Id == replyId);
        if (reply == null)
        {
            return false;
        }

        _context.Replies.Remove(reply);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ForumThread>> GetAllThreadsWithViolationsAsync()
    {
        return await _context.Threads
            .Include(t => t.Replies)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}

