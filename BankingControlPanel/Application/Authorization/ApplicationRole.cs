using Microsoft.AspNetCore.Identity;

namespace BankingControlPanel.Application.Authorization
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }
}
