using Easy.Net.Starter.EntityFramework;
using Easy.Net.Starter.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Easy.Net.Starter.Api.Authentications
{
    public interface IUserService
    {
        Task<bool> IsValidUser(string email, string password);
        Task<Guid> GetIdByEmail(string email);
        Task<bool> AddUser(
            string password,
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            DateTime dateOfBirth,
            bool isAdmin,
            bool isLocked);
    }
    public class UserService(BaseDbContext context) : IUserService
    {
        private readonly BaseDbContext _context = context;

        public async Task<Guid> GetIdByEmail(string email)
        {
            var user = await _context.Users
               .Where(u => u.Email == email)
               .FirstOrDefaultAsync();

            if (user != null)
            {
                return user.Id;
            }

            return Guid.Empty;
        }

        public async Task<bool> IsValidUser(string email, string password)
        {
            var user = await _context.Users
               .Where(u => u.Email == email)
               .FirstOrDefaultAsync();

            if (user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> AddUser(
            string password,
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            DateTime dateOfBirth,
            bool isAdmin,
            bool isLocked)
        {
            try
            {
                var newUser = new User
                {
                    Email = email,
                    Password = password.HashPassword(),
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    DateOfBirth = DateOnly.FromDateTime(dateOfBirth),
                    IsAdmin = false,
                    IsLocked = isLocked
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
