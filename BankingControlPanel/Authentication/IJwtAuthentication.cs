namespace BankingControlPanel.Authentication
{
    public interface IJwtAuthentication
    {
        string GenerateToken(string email, IList<string> roles);
    }
}
