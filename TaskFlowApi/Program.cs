using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TaskFlowApi.Application.Interfaces.Auth;
using TaskFlowApi.Application.Interfaces.Repositories;
using TaskFlowApi.Application.Interfaces.Services;
using TaskFlowApi.Application.Services;
using TaskFlowApi.Middleware;
using TaskFlowApi.Application.Interfaces.Demo;
using TaskFlowApi.Infra.DemoSession;
using TaskFlowApi.Infra.Security;
using TaskFlowApi.Infra.Repository.Demo;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"Bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProjetoService, ProjetoService>();
builder.Services.AddScoped<ITarefaService, TarefaService>();

builder.Services.AddSingleton<DemoSeedFactory>();
builder.Services.AddSingleton<IDemoSessionService, DemoSessionStore>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentSessionService, CurrentSessionService>();


builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryDemo>();
builder.Services.AddScoped<IProjetoRepository, ProjetoRepositoryDemo>();
builder.Services.AddScoped<ITarefaRepository, TarefaRepositoryDemo>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateAudience = false,
            ValidateIssuer = false,
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();

app.ConfigureExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();