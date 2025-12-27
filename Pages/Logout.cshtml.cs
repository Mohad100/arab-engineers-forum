using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Fourm.Pages;

/// <summary>
/// Page model for user logout
/// </summary>
public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        // Sign out the user
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        // Redirect to home page
        return RedirectToPage("/Index");
    }
}
