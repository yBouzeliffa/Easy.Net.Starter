using Easy.Net.Starter.EntityFramework;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Easy.Net.Starter.Api.Authentications
{
    public class UserClaimsTransformation : IClaimsTransformation
    {
        private readonly BaseDbContext _context;

        public UserClaimsTransformation(BaseDbContext context)
        {
            _context = context;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var IdClaim = principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier);
            if (IdClaim == null)
                return principal;

            var Id = new Guid(IdClaim.Value);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == Id);

            if (user == null)
                return principal;

            var claimsIdentity = principal.Identity as ClaimsIdentity;
            if (claimsIdentity != null && claimsIdentity.IsAuthenticated)
            {
                claimsIdentity.AddClaim(new Claim("Email", user.Email));
                claimsIdentity.AddClaim(new Claim("FirstName", user.FirstName ?? string.Empty));
                claimsIdentity.AddClaim(new Claim("LastName", user.LastName ?? string.Empty));
                claimsIdentity.AddClaim(new Claim("PhoneNumber", user.PhoneNumber ?? string.Empty));
                claimsIdentity.AddClaim(new Claim("DateOfBirth", user.DateOfBirth?.ToString() ?? string.Empty));
                claimsIdentity.AddClaim(new Claim("RegistrationDate", user.RegistrationDate.ToString()));
                claimsIdentity.AddClaim(new Claim("IsAdmin", user.IsAdmin.ToString()));
                claimsIdentity.AddClaim(new Claim("IsLocked", user.IsLocked.ToString()));
            }

            return principal;
        }
    }
}
