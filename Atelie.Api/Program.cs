using Atelie.Api.Data;
using Atelie.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Registrar Services
builder.Services.AddScoped<FinanceiroService>();
builder.Services.AddScoped<MaterialService>();
builder.Services.AddScoped<EstoqueService>();
builder.Services.AddScoped<PecaProntaService>();
builder.Services.AddScoped<VendaService>();
builder.Services.AddScoped<EncomendaService>();
builder.Services.AddScoped<TodoListService>();

// DbContext + PostgreSQL usando appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AtelieDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") // front local
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendPolicy");

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
