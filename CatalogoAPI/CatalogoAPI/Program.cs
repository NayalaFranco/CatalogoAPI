using AutoMapper;
using CatalogoAPI.Context;
using CatalogoAPI.DTOs.Mappings;
using CatalogoAPI.Extensions;
using CatalogoAPI.Filters;
using CatalogoAPI.Logging;
using CatalogoAPI.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Registrando o servi�o de filtro
builder.Services.AddScoped<ApiLoggingFilter>();

// Registrando o servi�o da Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Registrando o servi�o do AutoMapper
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Registrando a Connection String
builder.Services.AddDbContext<CatalogoAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// IdentityUser � o usu�rio do identity, e o 
// IdentityRole trata perfis de usu�rio.
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
// O AddEntityFrameworkStores adiciona uma implementa��o
// do entity framework core do Identity para o
// context que � o AppDbContext
        .AddEntityFrameworkStores<CatalogoAPIContext>()
// O AddDefaultTokenProviders ele permite gerar tokens
// ALEAT�RIOS para quando houver mudan�a de email,
// precisar resetar uma senha, altera telefone, e para
// gerar uma autentica��o de de duas etapas.
// (Esse n�o tem a ver com o JWT que estamos implementando)
        .AddDefaultTokenProviders();

// JWT
// Adiciona o manipulador de autentica��o e define o
// esquema de autentica��o usado : Bearer
// valida o emissor, a audiencia e a chave
// usando a chave secreta valida a assinatura
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidAudience = builder.Configuration["TokenConfiguration:Audience"],
            ValidIssuer = builder.Configuration["TokenConfiguration:Issuer"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))
        });

builder.Services.AddControllers()
    // Adicional para resolver o problema da referencia ciclica
    // onde categoria referencia produto, e produto referencia categoria
    // e a serializa��o entra num loop de referencia.
    .AddJsonOptions(options =>
        options.JsonSerializerOptions
            .ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ativando o logging
// adiciona o provider e sua configura��o
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    // atribui o nivel de log para information
    LogLevel = LogLevel.Information
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Chamando a extens�o do middleware
app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
