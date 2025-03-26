using Easy.Net.Starter.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Easy.Net.Starter.Services
{
    public interface IUserContextService
    {
        ConnectedUser GetConnectedUser();
    }
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ConnectedUser GetConnectedUser()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;

            if (identity == null || !identity.IsAuthenticated)
                throw new UnauthorizedAccessException("Utilisateur non authentifié.");

            return new ConnectedUser().FromClaims(identity);
        }
    }
}
