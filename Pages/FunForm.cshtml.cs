using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Fourm.Models;
using Fourm.Services;

namespace Fourm.Pages;

/// <summary>
/// Page model for the fun entertainment form
/// Only accessible to authenticated users
/// </summary>
[Authorize]
public class FunFormPageModel : PageModel
{
    private readonly IFunFormService _funFormService;

    [BindProperty]
    public FunFormModel Input { get; set; } = new();

    public FunFormPageModel(IFunFormService funFormService)
    {
        _funFormService = funFormService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Validate the model
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Set the username from the authenticated user
        Input.Username = User.Identity?.Name ?? "Anonymous";

        // Save the form submission
        await _funFormService.SaveFormAsync(Input);

        // Store data in TempData to show on success page
        TempData["Nickname"] = Input.Nickname;
        TempData["MoodToday"] = Input.MoodToday;

        // Redirect to success page
        return RedirectToPage("/Success");
    }
}
