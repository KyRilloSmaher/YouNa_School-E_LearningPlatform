using Serilog.Context;
using YouNaSchool.API.Extensions;

namespace YouNaSchool.API.Middlewares;

public class ModuleLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public ModuleLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Get module name from endpoint metadata
        var moduleName = context.GetEndpoint()?
                                .Metadata
                                .GetMetadata<ModuleAttribute>()?
                                .Name
                                ?? "General";

        using (LogContext.PushProperty("Module", moduleName))
        using (LogContext.PushProperty("TraceId", context.TraceIdentifier))
        {
            await _next(context);
        }
    }
}