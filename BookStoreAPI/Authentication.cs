using BookStoreAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI;

public static class Authentication
{
    public static async Task<CredentialsLevel> AuthenticateUser(string email, string password, DataContext _context)
    {
        try
        {
            var user = _context.Customers.First(c => c.Email == email && c.Password == password);
            return user.AccountType switch
            {
                0 => CredentialsLevel.Unauthorized,
                1 => CredentialsLevel.User,
                2 => CredentialsLevel.Admin,
                _ => CredentialsLevel.Unauthorized
            };
        }
        catch (Exception e)
        {
            return CredentialsLevel.Unauthorized;
        }
        
    }
    public enum CredentialsLevel
    {
        Admin,
        User,
        Unauthorized
    }
}