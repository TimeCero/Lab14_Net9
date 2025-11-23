using Lab07_AndreGonzales.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 1) Manejo global de errores: debe ir lo más arriba posible para atrapar excepciones de todo lo siguiente
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2) Redirección HTTPS (opcional pero recomendable)
app.UseHttpsRedirection();

// 3) Routing: necesario antes de middlewares que usan metadata/endpoint (ej: ParameterValidation)
app.UseRouting();

// 4) Control de roles: bloquea rápido antes de validar body (evita leer body si no tiene permiso)
app.UseMiddleware<RoleBasedAccessMiddleware>();

// 5) Validación de parámetros: necesita que UseRouting ya haya ejecutado para acceder al Endpoint metadata
app.UseMiddleware<ParameterValidationMiddleware>();

// 6) Swagger/UI (puedes dejarlo siempre activado o únicamente en Development)
app.UseSwagger();
app.UseSwaggerUI();

// 7) Mapear controladores (final del pipeline)
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();