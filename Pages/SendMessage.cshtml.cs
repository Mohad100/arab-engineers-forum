using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fourm.Services;
using System.ComponentModel.DataAnnotations;

namespace Fourm.Pages;

[Authorize]
public class SendMessageModel : PageModel
{
    private readonly IPrivateMessageService _messageService;
    private readonly IUserService _userService;

    public SendMessageModel(IPrivateMessageService messageService, IUserService userService)
    {
        _messageService = messageService;
        _userService = userService;
    }

    [BindProperty]
    [Required]
    public string RecipientUsername { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Subject must be between 1 and 200 characters")]
    public string Subject { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 5000 characters")]
    public string MessageContent { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string? recipient)
    {
        if (string.IsNullOrEmpty(recipient))
        {
            ErrorMessage = "No recipient specified";
            return Page();
        }

        // Verify recipient exists
        var recipientUser = await _userService.GetUserByUsernameAsync(recipient);
        if (recipientUser == null)
        {
            ErrorMessage = $"User '{recipient}' not found";
            return Page();
        }

        // Check if trying to message yourself
        if (recipientUser.Username.Equals(User.Identity?.Name, StringComparison.OrdinalIgnoreCase))
        {
            ErrorMessage = "You cannot send a message to yourself";
            return Page();
        }

        RecipientUsername = recipientUser.Username;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var senderUsername = User.Identity?.Name;
        if (string.IsNullOrEmpty(senderUsername))
        {
            ErrorMessage = "You must be logged in to send messages";
            return Page();
        }

        // Verify recipient exists
        var recipientUser = await _userService.GetUserByUsernameAsync(RecipientUsername);
        if (recipientUser == null)
        {
            ErrorMessage = $"User '{RecipientUsername}' not found";
            return Page();
        }

        // Check if trying to message yourself
        if (recipientUser.Username.Equals(senderUsername, StringComparison.OrdinalIgnoreCase))
        {
            ErrorMessage = "You cannot send a message to yourself";
            return Page();
        }

        try
        {
            await _messageService.SendAsync(senderUsername, recipientUser.Username, Subject, MessageContent);
            TempData["SuccessMessage"] = $"Message sent successfully to {recipientUser.Username}! 📬";
            return RedirectToPage("/Messages");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to send message: {ex.Message}";
            return Page();
        }
    }
}
