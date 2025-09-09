using AuthJwtApi.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthJwtApi.Services
{
    public class UserService : IUserService
    {
        // Simulação de "banco" em memória
        private static readonly List<User> _users = new();
        private static int _nextId = 1;
        private readonly PasswordHasher<User> _hasher = new();

        public UserService()
        {
            // opcional: criar usuários seed em desenvolvimento
            if (!_users.Any())
            {
                var u = new User { Id = _nextId++, Username = "admin", Email = "admin@example.com" };
                u.PasswordHash = _hasher.HashPassword(u, "Admin@123");
                _users.Add(u);
            }
        }

        public User Create(User user, string password)
        {
            if (_users.Any(x => x.Username?.ToLower() == user.Username?.ToLower()))
                throw new InvalidOperationException("Username already taken");

            user.Id = _nextId++;
            user.PasswordHash = _hasher.HashPassword(user, password);
            _users.Add(user);
            return user;
        }

        public IEnumerable<User> GetAll() => _users;

        public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public User? GetByUsername(string username) => _users.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));

        public bool VerifyPassword(User user, string password)
        {
            if (user == null || user.PasswordHash == null) return false;
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
