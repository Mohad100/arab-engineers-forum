using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fourm.Models;
using Fourm.Services;

namespace Fourm.Pages;

[Authorize]
public class MessagesModel : PageModel
{
    private readonly IPrivateMessageService _messageService;

    public MessagesModel(IPrivateMessageService messageService)
    {
        _messageService = messageService;
    }

    public List<PrivateMessage> InboxMessages { get; set; } = new();
    public List<PrivateMessage> SentMessages { get; set; } = new();
    public int UnreadCount { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToPage("/Login");
        }

        InboxMessages = await _messageService.GetInboxAsync(username);
        SentMessages = await _messageService.GetSentAsync(username);
        UnreadCount = await _messageService.GetUnreadCountAsync(username);

        return Page();
    }

    public async Task<IActionResult> OnPostMarkAsReadAsync(string messageId)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToPage("/Login");
        }

        var message = await _messageService.GetMessageAsync(messageId, username);
        if (message != null && message.RecipientUsername.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            await _messageService.MarkAsReadAsync(messageId);
            TempData["SuccessMessage"] = "Message marked as read";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string messageId)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToPage("/Login");
        }

        await _messageService.DeleteAsync(messageId, username);
        TempData["SuccessMessage"] = "Message deleted";

        return RedirectToPage();
    }
}
