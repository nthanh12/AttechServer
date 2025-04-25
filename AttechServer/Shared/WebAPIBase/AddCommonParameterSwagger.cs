using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AttechServer.Shared.WebAPIBase
{
    public class AddCommonParameterSwagger : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();
            var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor != null)
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "permission",
                    In = ParameterLocation.Query,
                    Description = "Dùng cho việc check permission trong một api gọi lại nhiều nơi",
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });
            }
        }
    }
}
