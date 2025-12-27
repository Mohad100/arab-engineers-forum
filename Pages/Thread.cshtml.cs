using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fourm.Models;
using Fourm.Services;

namespace Fourm.Pages;

/// <summary>
/// Page for viewing a specific thread and its replies
/// </summary>
public class ThreadPageModel : PageModel
{
    private readonly IForumService _forumService;
    private readonly IUserService _userService;

    public ForumThread? Thread { get; set; }
    public List<ThreadReply> Replies { get; set; } = new();
    public bool IsAdmin { get; set; } = false;

    [BindProperty]
    public CreateReplyModel ReplyInput { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public ThreadPageModel(IForumService forumService, IUserService userService)
    {
        _forumService = forumService;
        _userService = userService;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Thread = await _forumService.GetThreadByIdAsync(id);
        
        if (Thread == null)
        {
            return Page();
        }

        Replies = await _forumService.GetThreadRepliesAsync(id);
        
        // Check if current user is admin
        var currentUser = await _userService.GetUserByUsernameAsync(User.Identity?.Name ?? "");
        IsAdmin = currentUser?.IsAdmin ?? false;
        
        return Page();
    }

    public async Task<IActionResult> OnPostCreateReplyAsync(Guid id)
    {
        Thread = await _forumService.GetThreadByIdAsync(id);
        
        if (Thread == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            Replies = await _forumService.GetThreadRepliesAsync(id);
            
            // Re-check admin status
            var currentUser = await _userService.GetUserByUsernameAsync(User.Identity?.Name ?? "");
            IsAdmin = currentUser?.IsAdmin ?? false;
            
            return Page();
        }

        if (Thread.IsLocked)
        {
            ErrorMessage = "This thread is locked and cannot accept new replies.";
            Replies = await _forumService.GetThreadRepliesAsync(id);
            
            // Re-check admin status
            var currentUser = await _userService.GetUserByUsernameAsync(User.Identity?.Name ?? "");
            IsAdmin = currentUser?.IsAdmin ?? false;
            
            return Page();
        }

        var username = User.Identity?.Name ?? "Anonymous";
        
        // Handle file upload
        string? attachmentUrl = null;
        string? attachmentFileName = null;
        
        if (ReplyInput.Attachment != null && ReplyInput.Attachment.Length > 0)
        {
            // Validate file size (5MB max)
            if (ReplyInput.Attachment.Length > 5 * 1024 * 1024)
            {
                ErrorMessage = "حجم الملف يجب أن يكون أقل من 5 ميجابايت";
                Replies = await _forumService.GetThreadRepliesAsync(id);
                var currentUser = await _userService.GetUserByUsernameAsync(User.Identity?.Name ?? "");
                IsAdmin = currentUser?.IsAdmin ?? false;
                return Page();
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{ReplyInput.Attachment.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await ReplyInput.Attachment.CopyToAsync(fileStream);
            }

            attachmentUrl = $"/uploads/{uniqueFileName}";
            attachmentFileName = ReplyInput.Attachment.FileName;
        }
        
        await _forumService.CreateReplyAsync(id, ReplyInput.Content, username, attachmentUrl, attachmentFileName);

        TempData["SuccessMessage"] = "Reply posted successfully! 🎉";
        return RedirectToPage("/Thread", new { id });
    }

    public async Task<IActionResult> OnPostDeleteReplyAsync(Guid id, Guid replyId)
    {
        var username = User.Identity?.Name ?? "";
        var success = await _forumService.DeleteReplyAsync(replyId, username);

        if (success)
        {
            TempData["SuccessMessage"] = "Reply deleted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Unable to delete reply.";
        }

        return RedirectToPage("/Thread", new { id });
    }

    public async Task<IActionResult> OnPostEditReplyAsync(Guid id, Guid replyId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            TempData["ErrorMessage"] = "Reply content cannot be empty.";
            return RedirectToPage("/Thread", new { id });
        }

        var username = User.Identity?.Name ?? "";
        var success = await _forumService.UpdateReplyAsync(replyId, content, username);

        if (success)
        {
            TempData["SuccessMessage"] = "Reply updated successfully! ✨";
        }
        else
        {
            TempData["ErrorMessage"] = "Unable to update reply.";
        }

        return RedirectToPage("/Thread", new { id });
    }

    public async Task<IActionResult> OnPostDeleteThreadAsync(Guid id)
    {
        var username = User.Identity?.Name ?? "";
        var success = await _forumService.DeleteThreadAsync(id, username);

        if (success)
        {
            TempData["SuccessMessage"] = "Thread deleted successfully.";
            return RedirectToPage("/Forum");
        }
        else
        {
            TempData["ErrorMessage"] = "Unable to delete thread.";
            return RedirectToPage("/Thread", new { id });
        }
    }
}
