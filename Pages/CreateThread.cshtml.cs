using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Fourm.Models;
using Fourm.Services;

namespace Fourm.Pages;

/// <summary>
/// Page for creating a new forum thread
/// Requires authentication
/// </summary>
[Authorize]
public class CreateThreadModel : PageModel
{
    private readonly IForumService _forumService;
    private readonly IWebHostEnvironment _environment;

    [BindProperty]
    public Fourm.Models.CreateThreadModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }
    
    public ForumCategory? PreselectedCategory { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public CreateThreadModel(IForumService forumService, IWebHostEnvironment environment)
    {
        _forumService = forumService;
        _environment = environment;
    }

    public void OnGet()
    {
        // If category is specified in URL, preselect it
        if (!string.IsNullOrEmpty(Category))
        {
            PreselectedCategory = ForumCategory.GetById(Category);
            if (PreselectedCategory != null)
            {
                Input.CategoryId = PreselectedCategory.Id;
            }
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Restore preselected category on post
        if (!string.IsNullOrEmpty(Category))
        {
            PreselectedCategory = ForumCategory.GetById(Category);
        }
        
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var username = User.Identity?.Name ?? "Anonymous";
        
        // Handle image upload
        string? imageUrl = null;
        if (Input.Image != null)
        {
            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(Input.Image.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("Input.Image", "Only image files (JPG, PNG, GIF) are allowed.");
                return Page();
            }

            // Validate file size (max 5MB)
            if (Input.Image.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("Input.Image", "Image size must be less than 5MB.");
                return Page();
            }

            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "threads");
            Directory.CreateDirectory(uploadsPath);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Input.Image.CopyToAsync(stream);
            }

            imageUrl = $"/uploads/threads/{fileName}";
        }
        
        await _forumService.CreateThreadAsync(Input.CategoryId, Input.Title, Input.Content, username, imageUrl);

        TempData["SuccessMessage"] = "Thread created successfully! 🎉";
        
        // Redirect back to the category if it was specified
        if (!string.IsNullOrEmpty(Category))
        {
            return RedirectToPage("/Forum", new { category = Category });
        }
        
        return RedirectToPage("/Index");
    }
}
