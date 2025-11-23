using Microsoft.AspNetCore.Mvc.Controllers;
using System.Text.Json;


namespace Lab07_AndreGonzales.Middleware;

public class ParameterValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ParameterValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
        {
            var model = await DeserializeRequestBody(context);
            if (model == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Cuerpo de la solicitud inválido o vacío.");
                return;
            }

            var validationErrors = ValidateModel(model);
            if (validationErrors.Any())
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(string.Join("\n", validationErrors));
                return;
            }
        }

        await _next(context);
    }

    private async Task<object?> DeserializeRequestBody(HttpContext context)
    {
        if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
        {
            context.Request.EnableBuffering(); // ✅ permite leer varias veces
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // ✅ resetea el stream

            var dtoType = context.GetEndpoint()?.Metadata
                ?.OfType<ControllerActionDescriptor>()
                .FirstOrDefault()?.MethodInfo?.GetParameters()
                .FirstOrDefault()?.ParameterType;

            if (!string.IsNullOrWhiteSpace(body) && dtoType != null)
            {
                return JsonSerializer.Deserialize(body, dtoType, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        return null;
    }

    private List<string> ValidateModel(object model)
    {
        var errors = new List<string>();
        foreach (var property in model.GetType().GetProperties())
        {
            var value = property.GetValue(model);
            if (value == null)
            {
                errors.Add($"El parámetro '{property.Name}' es obligatorio.");
            }
        }
        return errors;
    }
}
