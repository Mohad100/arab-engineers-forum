using Microsoft.AspNetCore.Mvc.RazorPages;
using Fourm.Models;
using Fourm.Services;

namespace Fourm.Pages;

public class IndexModel : PageModel
{
    private readonly IForumService _forumService;

    public List<ForumThread> RecentThreads { get; set; } = new();
    public Dictionary<string, int> CategoryThreadCounts { get; set; } = new();
    public Dictionary<string, ForumThread?> LatestThreadsByCategory { get; set; } = new();

    public IndexModel(IForumService forumService)
    {
        _forumService = forumService;
    }

    public async Task OnGetAsync()
    {
        // Get all threads for recent activity
        RecentThreads = await _forumService.GetAllThreadsAsync();

        // Calculate thread counts per category
        foreach (var category in ForumCategory.AllCategories)
        {
            var categoryThreads = await _forumService.GetThreadsByCategoryAsync(category.Id);
            CategoryThreadCounts[category.Id] = categoryThreads.Count;
            LatestThreadsByCategory[category.Id] = categoryThreads.FirstOrDefault();
        }
    }
}
