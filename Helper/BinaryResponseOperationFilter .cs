using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Netflix_BackendAPI.Helper
{
    public class BinaryResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor descriptor &&
                descriptor.ControllerName == "Features" &&
                descriptor.ActionName == "StreamVideo")
            {
                operation.Responses["200"] = new OpenApiResponse
                {
                    Description = "Binary video stream",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/octet-stream"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            }
                        }
                    }
                };
            }
        }
    }
}