using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Easy.Net.Starter.Swagger
{
    public class AddReferrerHeaderFilter : IOperationFilter
    {
        private readonly string _defaultHeaderName;
        private readonly string _defaultValue;

        public AddReferrerHeaderFilter(string defaultHeaderName, string defaultValue)
        {
            _defaultHeaderName = defaultHeaderName;
            _defaultValue = defaultValue;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = _defaultHeaderName,
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema { Default = new OpenApiString(_defaultValue, true), Type = "string" }
            });
        }
    }
}
