using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Fourm.Models;
using Fourm.Services;

namespace Fourm.Pages;

/// <summary>
/// Page displaying all forum threads (optionally filtered by category)
/// </summary>
public class ForumModel : PageModel
{
    private readonly IForumService _forumService;

    public List<ForumThread> Threads { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }
    
    public ForumCategory? CurrentCategory { get; set; }

    public ForumModel(IForumService forumService)
    {
        _forumService = forumService;
    }

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrEmpty(Category))
        {
            CurrentCategory = ForumCategory.GetById(Category);
            Threads = await _forumService.GetThreadsByCategoryAsync(Category);
        }
        else
        {
            Threads = await _forumService.GetAllThreadsAsync();
        }
        
        // Ensure proper sorting: Pinned threads first, then by most recent activity
        Threads = Threads
            .OrderByDescending(t => t.IsPinned)
            .ThenByDescending(t => t.LastReplyAt)
            .ToList();
    }
}
