using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Fourm.Pages;

/// <summary>
/// Success page shown after form submission
/// </summary>
[Authorize]
public class SuccessModel : PageModel
{
    public void OnGet()
    {
    }
}
