namespace Lab07_AndreGonzales.Middleware;

using Microsoft.AspNetCore.Http;
using System.Linq;


public class RoleBasedAccessMiddleware
{
    private readonly RequestDelegate _next;

    public RoleBasedAccessMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Permitir acceso libre a Swagger
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        var role = context.Request.Headers["Role"].FirstOrDefault();

        if (string.IsNullOrEmpty(role))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Acceso denegado. No se especificó rol.");
            return;
        }

        if (role == "Admin")
        {
            await _next(context);
            return;
        }

        if (role == "User")
        {
            if (context.Request.Method == HttpMethods.Get)
            {
                await _next(context);
                return;
            }

            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Acceso denegado. El rol 'User' no tiene permisos para este endpoint.");
            return;
        }

        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Acceso denegado. Rol no válido.");
    }
}