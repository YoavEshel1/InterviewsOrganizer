using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace InterviewsOrganizer.Swagger
{
    /// <summary>
    /// Adds the "Bearer" security requirement to Swagger operations that require authorization,
    /// based on [Authorize]/[AllowAnonymous] metadata, so the lock icon and 401/403 responses
    /// only show up where they actually apply.
    /// </summary>
    public class AuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<AuthorizeAttribute>()
                .Any();
            var hasAllowAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<AllowAnonymousAttribute>()
                .Any();

            if (!hasAuthorize || hasAllowAnonymous)
                return;

            var bearerScheme = new OpenApiSecuritySchemeReference("Bearer", context.Document);

            var requirement = new OpenApiSecurityRequirement();
            requirement[bearerScheme] = [];
            operation.Security ??= [];
            operation.Security.Add(requirement);

            operation.Responses ??= [];
            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
        }
    }
}