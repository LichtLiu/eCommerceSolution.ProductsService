namespace APILayer.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    // RequestDelegate:pipeline 裡的「下一個」處理者,由框架注入
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            // 放行給後面的 middleware / endpoint;後面任何地方丟例外都會被這裡接住
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            // 完整例外(含 stack trace)只寫進 log,不回給 client
            _logger.LogError(ex, "Unhandled exception occurred while processing {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(new
            {
                error = "An unexpected error occurred. Please try again later."
            });
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// 把 ExceptionHandlingMiddleware 加進 request pipeline。
    /// </summary>
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
