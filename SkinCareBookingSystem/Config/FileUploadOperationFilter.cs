using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace SkinCareBookingSystem.Config
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check if any parameter has IFormFile
            var fileParam = context.ApiDescription.ActionDescriptor.Parameters
                .FirstOrDefault(p => p.ParameterType == typeof(IFormFile));

            if (fileParam != null)
            {
                // Modify the Swagger schema for file parameters to be of type "string" with "binary" format
                var fileParamSwagger = operation.Parameters.FirstOrDefault(p => p.Name == fileParam.Name);
                if (fileParamSwagger != null)
                {
                    fileParamSwagger.Schema.Type = "string";
                    fileParamSwagger.Schema.Format = "binary";
                }
            }
        }
    }
}
