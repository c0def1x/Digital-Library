using Digital_Library.Domain.Entities;

namespace Digital_Library.Domain.Services
{
    public interface IUserService
    {
        Task<bool> IsUserExistsAsync (string username);
        Task<User> RegistrationAsync (string fullname, string username, string password);
        Task<User?> GetUserAsync (string username, string password);
    }
}
