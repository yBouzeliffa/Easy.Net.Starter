using Easy.Net.Starter.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Easy.Net.Starter.Api.Authentications
{
    public class IsAdminAttribute : TypeFilterAttribute
    {
        public IsAdminAttribute() : base(typeof(ClaimRequirementFilter))
        {
            var claims = new List<Claim> { new Claim("IsAdmin", "True") };
            Arguments = new object[] { claims };
        }
    }

    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly List<Claim> _claims;

        public ClaimRequirementFilter(List<Claim> claims)
        {
            _claims = claims;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userClaims = context.HttpContext.User.Claims.ToList();

            bool hasRequiredClaim = _claims.Any(requiredRole => userClaims.Any(uc => uc.Type == requiredRole.Type && uc.Value == requiredRole.Value));

            if (!hasRequiredClaim)
            {
                throw new BusinessException("Accès non autorisé", "Accès non autorisé", "403");
            }
        }
    }
}
