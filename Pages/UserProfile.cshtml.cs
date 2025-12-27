using Microsoft.AspNetCore.Mvc.RazorPages;
using Fourm.Models;
using Fourm.Services;

namespace Fourm.Pages;

public class UserProfileModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IForumService _forumService;
    private readonly IPrivateMessageService _messageService;

    public UserProfileModel(IUserService userService, IForumService forumService, IPrivateMessageService messageService)
    {
        _userService = userService;
        _forumService = forumService;
        _messageService = messageService;
    }

    public User? ProfileUser { get; set; }
    public List<ForumThread> UserThreads { get; set; } = new();
    public int ThreadCount { get; set; }
    public int ReplyCount { get; set; }
    public int UnreadMessageCount { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(string? username)
    {
        var currentUserId = User.FindFirst("UserId")?.Value ?? "";
        var currentUsername = User.FindFirst("UserName")?.Value ?? "";

        if (string.IsNullOrEmpty(username))
        {
            ErrorMessage = "User not found.";
            return;
        }

        ProfileUser = await _userService.GetUserByUsernameAsync(username);
        
        if (ProfileUser == null)
        {
            ErrorMessage = "User not found.";
            return;
        }

        // Get user's threads
        var allThreads = await _forumService.GetAllThreadsAsync();
        UserThreads = allThreads
            .Where(t => t.AuthorUsername == username)
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .ToList();

        ThreadCount = allThreads.Count(t => t.AuthorUsername == username);
        
        // Count replies by this user
        ReplyCount = allThreads
            .SelectMany(t => t.Replies)
            .Count(r => r.AuthorUsername == username);

        // Get unread count for current user
        if (!string.IsNullOrEmpty(currentUserId))
        {
            UnreadMessageCount = await _messageService.GetUnreadCountAsync(currentUserId);
        }
    }
}
