using Fourm.Models;

namespace Fourm.Services;

/// <summary>
/// Interface for fun form submission service
/// </summary>
public interface IFunFormService
{
    Task SaveFormAsync(FunFormModel form);
    Task<List<FunFormModel>> GetAllFormsAsync();
    Task<List<FunFormModel>> GetUserFormsAsync(string username);
}
