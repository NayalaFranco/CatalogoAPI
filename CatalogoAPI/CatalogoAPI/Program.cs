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
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Seguindo a documentação do net 6 sobre CORS
var NomeDaPoliticaCors = "NomeQualquerDaPoliticaCors";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: NomeDaPoliticaCors,
                        policy =>
                        {
                            policy.WithOrigins("https://www.apirequest.io",
                                "https://apirequest.io")
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                        }
                      );
});

// Add services to the container.

// Registrando o serviço de filtro
builder.Services.AddScoped<ApiLoggingFilter>();

// Registrando o serviço da Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Registrando o serviço do AutoMapper
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Registrando a Connection String
builder.Services.AddDbContext<CatalogoAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// IdentityUser é o usuário do identity, e o 
// IdentityRole trata perfis de usuário.
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
// O AddEntityFrameworkStores adiciona uma implementação
// do entity framework core do Identity para o
// context que é o AppDbContext
        .AddEntityFrameworkStores<CatalogoAPIContext>()
// O AddDefaultTokenProviders ele permite gerar tokens
// ALEATÓRIOS para quando houver mudança de email,
// precisar resetar uma senha, altera telefone, e para
// gerar uma autenticação de de duas etapas.
// (Esse não tem a ver com o JWT que estamos implementando)
        .AddDefaultTokenProviders();

// JWT
// Adiciona o manipulador de autenticação e define o
// esquema de autenticação usado : Bearer
// valida o emissor, a audiência e a chave
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
    // Adicional para resolver o problema da referencia cíclica
    // onde categoria referencia produto, e produto referencia categoria
    // e a serialização entra num loop de referencia.
    .AddJsonOptions(options =>
        options.JsonSerializerOptions
            .ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CatalogoAPI", Version = "v1" });

    // Informa o tipo de segurança usado e uma descrição de
    // como inserir ela na janelinha que isso vai criar no swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Header de autorização JWT usando o esquema Bearer." +
        "\r\n\r\nInforme 'Bearer'[espaço] e o seu token." +
        "\r\n\r\nExemplo: \'Bearer 12345abcdef\'",
    });

    // Informa o tipo de segurança requerido
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type= ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Ativando o logging
// adiciona o provider e sua configuração
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    // atribui o nível de log para information
    LogLevel = LogLevel.Information
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Chamando a extensão do middleware
app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(NomeDaPoliticaCors);

app.UseAuthorization();

app.MapControllers();

app.Run();
