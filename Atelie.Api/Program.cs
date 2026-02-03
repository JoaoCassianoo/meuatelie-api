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

// DbContext + SQLite
builder.Services.AddDbContext<AtelieDbContext>(options =>
    options.UseSqlite("Data Source=atelie.db")
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendPolicy");

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
