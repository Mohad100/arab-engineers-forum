using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Fourm.Models;
using Fourm.Services;
using System.ComponentModel.DataAnnotations;

namespace Fourm.Pages;

[Authorize]
public class EditThreadModel : PageModel
{
    private readonly IForumService _forumService;
    private readonly IWebHostEnvironment _environment;

    public class InputModel
    {
        [Required(ErrorMessage = "Thread title is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Thread content is required")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 5000 characters")]
        public string Content { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public string? CurrentImageUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public EditThreadModel(IForumService forumService, IWebHostEnvironment environment)
    {
        _forumService = forumService;
        _environment = environment;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var thread = await _forumService.GetThreadByIdAsync(Id);
        
        if (thread == null)
        {
            return NotFound();
        }

        // Check if the current user is the author
        if (thread.AuthorUsername != User.Identity?.Name)
        {
            return Forbid();
        }

        Input.Title = thread.Title;
        Input.Content = thread.Content;
        CurrentImageUrl = thread.ImageUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var thread = await _forumService.GetThreadByIdAsync(Id);
            if (thread != null)
            {
                CurrentImageUrl = thread.ImageUrl;
            }
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
                var thread = await _forumService.GetThreadByIdAsync(Id);
                if (thread != null)
                {
                    CurrentImageUrl = thread.ImageUrl;
                }
                return Page();
            }

            // Validate file size (max 5MB)
            if (Input.Image.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("Input.Image", "Image size must be less than 5MB.");
                var thread = await _forumService.GetThreadByIdAsync(Id);
                if (thread != null)
                {
                    CurrentImageUrl = thread.ImageUrl;
                }
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

        var success = await _forumService.UpdateThreadAsync(Id, Input.Title, Input.Content, username, imageUrl);

        if (!success)
        {
            ErrorMessage = "Unable to update thread. You may not have permission.";
            return Page();
        }

        TempData["SuccessMessage"] = "Thread updated successfully! ✨";
        return RedirectToPage("/Thread", new { id = Id });
    }
}
