using BankingControlPanel.Application.Services.Users.Dto;
using Microsoft.AspNetCore.Identity;

namespace BankingControlPanel.Application.Services.Users
{
    public interface IUserService 
    {
        Task<IdentityResult> RegisterUserAsync(RegisterModel model, string role);
        Task<SignInResult> LoginUserAsync(LoginModel model);
        Task<IList<string>> GetUserRolesAsync(string email);
    }
}
