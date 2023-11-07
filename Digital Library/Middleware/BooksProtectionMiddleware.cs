namespace Digital_Library.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class BooksProtectionMiddleware
    {
        private readonly RequestDelegate _next;

        public BooksProtectionMiddleware (RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke (HttpContext httpContext)
        {
            if (httpContext.Request.Path.ToString().StartsWith("/books/"))
            {
                if (httpContext.User.Identity?.IsAuthenticated != true)
                {
                    httpContext.Response.Redirect("/user/login/");
                    return Task.CompletedTask;
                }
            }
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class BooksProtectionMiddlewareExtensions
    {
        public static IApplicationBuilder UseBooksProtection (this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BooksProtectionMiddleware>();
        }
    }
}
