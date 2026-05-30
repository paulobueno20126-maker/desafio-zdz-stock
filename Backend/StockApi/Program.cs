var builder = WebApplication.CreateBuilder(args);

// --- ATIVAR PERMISSÃO DE CONEXÃO (CORS) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// --- UTILIZAR A CONFIGURAÇÃO DE CORS ---
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();