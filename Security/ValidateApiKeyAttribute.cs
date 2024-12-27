using Easy.Net.Starter.Models;
using Easy.Net.Starter.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Easy.Net.Starter.Security
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class ValidateApiKeyAttribute : ActionFilterAttribute
    {
        private IRsaService _rsaService;
        private AppSettings _appSettings;

        public ValidateApiKeyAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _rsaService = (IRsaService)context.HttpContext.RequestServices.GetService(typeof(IRsaService));
            _appSettings = (AppSettings)context.HttpContext.RequestServices.GetService(typeof(AppSettings));

            ArgumentNullException.ThrowIfNull(_rsaService, nameof(_rsaService));
            ArgumentNullException.ThrowIfNull(_appSettings, nameof(_appSettings));

            base.OnActionExecuting(context);

            if (!IsValidApiKey(context.HttpContext.Request))
            {
                context.Result = new ContentResult
                {
                    Content = "Invalid Api Key",
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    ContentType = "text/plain"
                };
            }
        }

        private bool IsValidApiKey(HttpRequest request)
        {
            if (!request.Headers.ContainsKey("X-API-KEY"))
            {
                return false;
            }

            string publicApiKey = request.Headers["X-API-KEY"];

            if (string.IsNullOrWhiteSpace(publicApiKey))
            {
                return false;
            }

            if (!_appSettings.ApiKey.ApiPublicKey.Contains(publicApiKey))
            {
                return false;
            }

            byte[] signature = _rsaService.CreateApiKey();
            bool isValid = _rsaService.ValidateApiKey(signature, _appSettings.ApiKey.ApiPublicKey.Replace("\\r", "\r").Replace("\\n", "\n"));

            if (isValid)
            {
                return true;
            }

            return false;
        }
    }
}
