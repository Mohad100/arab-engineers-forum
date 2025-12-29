using Microsoft.AspNetCore.Mvc.RazorPages;
using Fourm.Models;
using Fourm.Services;

namespace Fourm.Pages;

public class IndexModel : PageModel
{
    private readonly IForumService _forumService;
    private readonly IUserService _userService;

    public List<ForumThread> RecentThreads { get; set; } = new();
    public List<ForumThread> TrendingThreads { get; set; } = new();
    public List<ForumThread> HotTopics { get; set; } = new();
    public Dictionary<string, int> CategoryThreadCounts { get; set; } = new();
    public Dictionary<string, ForumThread?> LatestThreadsByCategory { get; set; } = new();
    public int TotalUsers { get; set; }
    public int ThreadsToday { get; set; }
    public List<User> TopContributors { get; set; } = new();

    public IndexModel(IForumService forumService, IUserService userService)
    {
        _forumService = forumService;
        _userService = userService;
    }

    public async Task OnGetAsync()
    {
        // Get all threads for recent activity
        var allThreads = await _forumService.GetAllThreadsAsync();
        RecentThreads = allThreads.OrderByDescending(t => t.CreatedAt).Take(5).ToList();

        // Trending threads (most replies in last 7 days)
        var weekAgo = DateTime.UtcNow.AddDays(-7);
        TrendingThreads = allThreads
            .Where(t => t.CreatedAt >= weekAgo)
            .OrderByDescending(t => t.ReplyCount)
            .ThenByDescending(t => t.CreatedAt)
            .Take(5)
            .ToList();

        // Hot topics (recent activity)
        HotTopics = allThreads
            .Where(t => t.Replies.Any())
            .OrderByDescending(t => t.Replies.Max(r => r.CreatedAt))
            .Take(5)
            .ToList();

        // Calculate thread counts per category
        foreach (var category in ForumCategory.AllCategories)
        {
            var categoryThreads = await _forumService.GetThreadsByCategoryAsync(category.Id);
            CategoryThreadCounts[category.Id] = categoryThreads.Count;
            LatestThreadsByCategory[category.Id] = categoryThreads.FirstOrDefault();
        }

        // Get stats
        var allUsers = await _userService.GetAllUsersAsync();
        TotalUsers = allUsers.Count;
        ThreadsToday = allThreads.Count(t => t.CreatedAt.Date == DateTime.UtcNow.Date);

        // Top contributors (most threads + replies)
        TopContributors = allUsers
            .Select(u => new
            {
                User = u,
                ThreadCount = allThreads.Count(t => t.AuthorUsername == u.Username),
                ReplyCount = allThreads.SelectMany(t => t.Replies).Count(r => r.AuthorUsername == u.Username)
            })
            .OrderByDescending(x => x.ThreadCount + x.ReplyCount)
            .Take(5)
            .Select(x => x.User)
            .ToList();
    }
}
