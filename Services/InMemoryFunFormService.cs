using Fourm.Models;

namespace Fourm.Services;

/// <summary>
/// In-memory storage for fun form submissions
/// In a real application, this would be replaced with a database
/// </summary>
public class InMemoryFunFormService : IFunFormService
{
    private readonly List<FunFormModel> _forms = new();
    private readonly object _lock = new();

    public Task SaveFormAsync(FunFormModel form)
    {
        lock (_lock)
        {
            form.SubmittedAt = DateTime.Now;
            _forms.Add(form);
        }
        return Task.CompletedTask;
    }

    public Task<List<FunFormModel>> GetAllFormsAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_forms.OrderByDescending(f => f.SubmittedAt).ToList());
        }
    }

    public Task<List<FunFormModel>> GetUserFormsAsync(string username)
    {
        lock (_lock)
        {
            return Task.FromResult(_forms
                .Where(f => f.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(f => f.SubmittedAt)
                .ToList());
        }
    }
}
