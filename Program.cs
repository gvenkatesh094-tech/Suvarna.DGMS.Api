using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Suvarna.DGMS.Api.Data;
using Suvarna.DGMS.Api.Filters;
using Suvarna.DGMS.Api.Middleware;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;
using Suvarna.DGMS.Api.Services;
using Suvarna.DGMS.Api.Helpers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

#endregion

#region Controllers

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelAttribute>();
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

#endregion

#region Swagger

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Suvarna DGMS API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id="Bearer",
                    Type=ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

#region Database

builder.Services.AddSingleton<DbHelper>();

builder.Services.AddScoped<ErrorRepository>();
builder.Services.AddScoped<PropertyRepository>();
builder.Services.AddScoped<BuildingRepository>();
builder.Services.AddScoped<FloorRepository>();
builder.Services.AddScoped<AuthRepository>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<RoomRepository>();
builder.Services.AddScoped<BedRepository>();

builder.Services.AddScoped<AccomidationTypeRepository>();
#endregion

#region Services


builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<PropertyService>();

builder.Services.AddScoped<BuildingService>();

builder.Services.AddScoped<FloorService>();

builder.Services.AddScoped<AccomidationTypeService>();
builder.Services.AddScoped<BedService>();
builder.Services.AddScoped<RoomService>();

#endregion

#region Form Size

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024;
});

#endregion

#region JWT

var jwtSection = builder.Configuration.GetSection("Jwt");

builder.Services.Configure<JwtSettings>(jwtSection);

var jwtSettings = jwtSection.Get<JwtSettings>()
                  ?? throw new Exception("JWT Settings Missing");

var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,

        IssuerSigningKey = new SymmetricSecurityKey(key),

        ClockSkew = TimeSpan.Zero
    };

    options.Events = new CustomAuthFailureHandler();
});
builder.Services.Configure<ModuleSettings>(builder.Configuration.GetSection("ModuleSettings"));

builder.Services.Configure<SessionSettings>(builder.Configuration.GetSection("SessionSettings"));

builder.Services.AddAuthorization();

#endregion

var app = builder.Build();

#region Middleware

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseMiddleware<RequestLoggingMiddleware>();



app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Suvarna DGMS API V1");

    c.RoutePrefix = "swagger";
});

app.MapControllers();

#endregion

app.Run();