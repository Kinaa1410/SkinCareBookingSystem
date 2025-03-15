using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace SkinCareBookingSystem.Config
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParams = context.ApiDescription.ParameterDescriptions
                .Where(p => p.Type == typeof(IFormFile))
                .ToList();

            foreach (var param in fileParams)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = param.Name,
                    In = ParameterLocation.Header,  // This is how you handle form-data in Swagger now
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"  // Specify the binary format for file uploads
                    }
                });
            }
        }
    }
}
