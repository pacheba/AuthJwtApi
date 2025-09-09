using AuthJwtApi.Models;

namespace AuthJwtApi.Services
{
    public interface IUserService
    {
        User? GetByUsername(string username);
        User? GetById(int id);
        IEnumerable<User> GetAll();
        User Create(User user, string password);
        bool VerifyPassword(User user, string password);
    }
}
