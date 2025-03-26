using System.Security.Claims;

namespace Easy.Net.Starter.Models
{
    public class ConnectedUser
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public DateTime RegistrationDate { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsLocked { get; set; }

        public string DisplayName => $"{FirstName} {LastName}".Trim();

        public ConnectedUser FromClaims(ClaimsIdentity identity)
        {
            var user = new ConnectedUser
            {
                Email = identity.FindFirst("Email")?.Value ?? string.Empty,
                FirstName = identity.FindFirst("FirstName")?.Value,
                LastName = identity.FindFirst("LastName")?.Value,
                PhoneNumber = identity.FindFirst("PhoneNumber")?.Value,
                IsAdmin = bool.TryParse(identity.FindFirst("IsAdmin")?.Value, out var isAdmin) && isAdmin,
                IsLocked = bool.TryParse(identity.FindFirst("IsLocked")?.Value, out var isLocked) && isLocked,
                RegistrationDate = DateTime.TryParse(identity.FindFirst("RegistrationDate")?.Value, out var regDate) ? regDate : default
            };

            if (DateOnly.TryParse(identity.FindFirst("DateOfBirth")?.Value, out var dateOfBirth))
                user.DateOfBirth = dateOfBirth;

            if (Guid.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var Id))
                user.Id = Id;

            return user;
        }
    }
}
