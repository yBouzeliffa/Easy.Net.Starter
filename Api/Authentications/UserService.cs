using Easy.Net.Starter.EntityFramework;
using Easy.Net.Starter.Extensions;
using Easy.Net.Starter.Models;
using Easy.Net.Starter.Services;
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

        Task<UserLight[]> GetAllUsersAsync(CancellationToken cancellationToken);
    }
    public class UserService(BaseDbContext context, IEmailService emailService, AppSettings appSettings) : IUserService
    {
        private readonly BaseDbContext _context = context;
        private readonly IEmailService emailService = emailService;
        private readonly AppSettings appSettings = appSettings;

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
                EmailValidation[] emailValidations = [];
                var emailValidation = new EmailValidation
                {
                    Token = Guid.NewGuid().ToString(),
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                };

                if (appSettings.EmailApiKey is not null)
                {
                    emailValidations = [emailValidation];
                }

                var newUser = new User
                {
                    Email = email,
                    Password = password.HashPassword(),
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    DateOfBirth = DateOnly.FromDateTime(dateOfBirth),
                    IsAdmin = false,
                    IsLocked = isLocked,
                    EmailValidations = emailValidations,
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                if (appSettings.EmailApiKey is not null)
                {
                    await emailService.SendWelcomeEmailAsync(email, firstName, appSettings.MobileAppRedirection);
                    await emailService.SendEmailValidationAsync(email, firstName, emailValidation.GenerateLink(appSettings.ApiBaseUrl));
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<UserLight[]> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            return await _context.Users
                .Select(x => new UserLight(x.Id, x.Email, x.LastName))
                .ToArrayAsync(cancellationToken);
        }
    }
}
