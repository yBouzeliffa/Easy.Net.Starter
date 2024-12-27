using Easy.Net.Starter.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace Easy.Net.Starter.Security
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class ValidateRefererAttribute : ActionFilterAttribute
    {
        private AppSettings appSettings;
        public ValidateRefererAttribute() { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            appSettings = (AppSettings)context.HttpContext.RequestServices.GetService(typeof(AppSettings));
            base.OnActionExecuting(context);
            if (!IsValidRequest(context.HttpContext.Request))
            {
                context.Result = new ContentResult
                {
                    Content = $"Invalid header"
                };
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
            }
        }

        private bool IsValidRequest(HttpRequest request)
        {
            if (!request.Headers.TryGetValue("Referer", out StringValues referrerURL))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(referrerURL)) return false;

            var urls = appSettings.AllowedOrigins.Split(',')?.Select(url => new Uri(url).Authority).ToList();
            var host = request.Host.Value;
            urls.Add(host);
            bool isValidClient = urls.Contains(new Uri(referrerURL).Authority);

            return isValidClient;
        }
    }
}
