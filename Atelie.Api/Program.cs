using Atelie.Api.Data;
using Atelie.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<FinanceiroService>();
builder.Services.AddScoped<MaterialService>();
builder.Services.AddScoped<EstoqueService>();
builder.Services.AddScoped<PecaProntaService>();
builder.Services.AddScoped<VendaService>();
builder.Services.AddScoped<EncomendaService>();
builder.Services.AddScoped<TodoListService>();
builder.Services.AddScoped<AtelieService>();
builder.Services.AddHttpClient<AbacatePayService>();
builder.Services.AddScoped<AssinaturaService>();

// HttpClient para Supabase
builder.Services.AddHttpClient();


// DbContext + PostgreSQL (Supabase Pooler)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var supabaseUrl = builder.Configuration["Supabase:Url"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = supabaseUrl + "auth/v1";
        options.TokenValidationParameters = new (){
            ValidateAudience = false,
            ValidIssuer = supabaseUrl + "auth/v1",
            ValidateIssuer = true,
            ValidateLifetime = true,
            NameClaimType = "sub"
        };
    });

builder.Services.AddDbContext<AtelieDbContext>(options =>
{
    options.UseNpgsql(
        connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null
            );
        });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthorization(options =>{
    options.FallbackPolicy = options.DefaultPolicy;
});

var app = builder.Build();

// Swagger em produção
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.UseMiddleware<PlanoAtivoMiddleware>();

app.MapControllers();

app.Run();
