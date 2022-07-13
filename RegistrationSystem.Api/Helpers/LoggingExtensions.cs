using Microsoft.AspNetCore.Diagnostics;
using RegistrationSystem.Api.Helpers.Exceptions;
using NLog.Extensions.Logging;

namespace RegistrationSystem.Api.Helpers
{
    public static class LogingExtensions
    {
        public static void SetupLogging(this WebApplicationBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            builder.Logging.AddNLog(config);
            builder.Logging.AddConsole();
        }

        public static void HandleUnhandledExceptions<TProgram>(this WebApplication app)
        {
            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async (context) =>
                {
                    context.Response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>() ?? throw new Exception();
                    var error = exceptionHandlerPathFeature.Error ?? throw new Exception();
                    var logger = context.RequestServices.GetService<ILogger<TProgram>>();
                    logger?.LogError(error, "Unhandeled Exception");
                    switch (error)
                    {
                        case HttpException:
                            var exp = (HttpException)error;
                            await exp.WriteToHttpContext(context);
                            break;
                        default:
                            throw error;
                    }
                });
            });
        }
    }
}