using Microsoft.AspNetCore.Mvc.RazorPages;
using Fourm.Models;
using Fourm.Services;

namespace Fourm.Pages;

public class StatsModel : PageModel
{
    private readonly IForumService _forumService;
    private readonly IUserService _userService;

    public int TotalUsers { get; set; }
    public int TotalThreads { get; set; }
    public int TotalReplies { get; set; }
    public int TotalCategories { get; set; }
    public List<User> RecentUsers { get; set; } = new();
    public List<ForumThread> RecentThreads { get; set; } = new();

    public StatsModel(IForumService forumService, IUserService userService)
    {
        _forumService = forumService;
        _userService = userService;
    }

    public async Task OnGetAsync()
    {
        // Get all data
        var allThreads = await _forumService.GetAllThreadsAsync();
        var allUsers = await _userService.GetAllUsersAsync();

        // Calculate statistics
        TotalUsers = allUsers.Count;
        TotalThreads = allThreads.Count;
        TotalReplies = allThreads.SelectMany(t => t.Replies).Count();
        TotalCategories = 10; // We have 10 engineering categories

        // Get recent data
        RecentUsers = allUsers.OrderByDescending(u => u.CreatedAt).ToList();
        RecentThreads = allThreads.OrderByDescending(t => t.CreatedAt).ToList();
    }
}
