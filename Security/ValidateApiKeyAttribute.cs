﻿using Easy.Net.Starter.Models;
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
        private IRsaService? _rsaService;
        private AppSettings? _appSettings;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var rsaObject = context.HttpContext.RequestServices.GetService(typeof(IRsaService));
            var appSettingsObject = context.HttpContext.RequestServices.GetService(typeof(AppSettings));

            ArgumentNullException.ThrowIfNull(rsaObject, nameof(rsaObject));
            ArgumentNullException.ThrowIfNull(appSettingsObject, nameof(appSettingsObject));

            _rsaService = (IRsaService)rsaObject;
            _appSettings = (AppSettings)appSettingsObject;

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
            if (!request.Headers.TryGetValue("X-API-KEY", out Microsoft.Extensions.Primitives.StringValues value))
            {
                return false;
            }

            string publicApiKey = value;

            if (string.IsNullOrWhiteSpace(publicApiKey))
            {
                return false;
            }

            ArgumentNullException.ThrowIfNull(_rsaService, nameof(_rsaService));
            ArgumentNullException.ThrowIfNull(_appSettings, nameof(_appSettings));

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
