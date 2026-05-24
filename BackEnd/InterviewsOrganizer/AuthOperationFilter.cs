using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace InterviewsOrganizer.Swagger
{
    public class AuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IOpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Required = false,
                Schema = new OpenApiSchema { Type = JsonSchemaType.String },
                Description = "Enter: Bearer {your JWT token}"
            });
        }
    }
}