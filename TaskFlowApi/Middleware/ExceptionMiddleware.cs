using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using TaskFlowApi.Domain.Enum;
using TaskFlowApi.Domain.Exceptions;

namespace TaskFlowApi.Middleware
{
    public static class ExceptionMiddleware
    {
        public static void ConfigureExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        int statusCode = 500; // default
                        string message = contextFeature.Error.Message;

                        if (contextFeature.Error is DomainException ex)
                        {
                            statusCode = ex.Type switch
                            {
                                ErrorTypeEnum.Validation => 400,
                                ErrorTypeEnum.NotFound => 404,
                                ErrorTypeEnum.Conflict => 409,
                                ErrorTypeEnum.Forbidden => 403,
                                ErrorTypeEnum.Unauthorized => 401,
                                _ => 500
                            };
                        }

                        context.Response.StatusCode = statusCode;

                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            status = statusCode,
                            message
                        }));
                    }
                });
            });
        }
    }
}
