using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Suvarna.DGMS.Api.Controllers;
using Suvarna.DGMS.Api.Data;
using Suvarna.DGMS.Api.Filters;
using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Middleware;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;
using Suvarna.DGMS.Api.Services;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region Logging (Serilog - daily log file)

builder.Host.UseSerilog((context, logConfig) => logConfig
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.File(
        Path.Combine(AppContext.BaseDirectory, "Logs", "dgms-api-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30));

#endregion

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
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

#region Repositories

builder.Services.AddSingleton<DbHelper>();

builder.Services.AddScoped<ErrorRepository>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<PropertyRepository>();
builder.Services.AddScoped<BuildingRepository>();
builder.Services.AddScoped<FloorRepository>();
builder.Services.AddScoped<RoomRepository>();
builder.Services.AddScoped<BedRepository>();
builder.Services.AddScoped<AccomidationTypeRepository>();
builder.Services.AddScoped<ServicesCatalogueRepository>();
builder.Services.AddScoped<RoomAmenitiesRepository>();
builder.Services.AddScoped<OccupantGuestRepository>();
builder.Services.AddScoped<SystemParametersRepository>();
builder.Services.AddScoped<DepartmentMappingRepository>();
builder.Services.AddScoped<CorpAgreementRepository>();
builder.Services.AddScoped<GstConfigRepository>();
builder.Services.AddScoped<TariffRepository>();
builder.Services.AddScoped<CheckinRepository>();
builder.Services.AddScoped<CheckoutRepository>();
builder.Services.AddScoped<HkDashboardRepository>();
builder.Services.AddScoped<LookupRepository>();
builder.Services.AddScoped<ReservationsRepository>();
builder.Services.AddScoped<GuestFolioBillingRepository>();
builder.Services.AddScoped<ActiveGuestsRepository>();
builder.Services.AddScoped<RoomTransferRepository>();
builder.Services.AddScoped<CorporateInvoicesRepository>();
builder.Services.AddScoped<WaitingListRepository>();
builder.Services.AddScoped<LaundryLogRepository>();
builder.Services.AddScoped<DepositLedgerRepository>();
builder.Services.AddScoped<GstReportsRepository>();
builder.Services.AddScoped<MaintenanceRequestsRepository>();
builder.Services.AddScoped<RevenueReportsRepository>();
builder.Services.AddScoped<ExecutiveDashboardRepository>();
builder.Services.AddScoped<OccupancyReportsRepository>();
builder.Services.AddScoped<ReceptionDashboardRepository>();

#endregion

#region Services

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PropertyService>();
builder.Services.AddScoped<BuildingService>();
builder.Services.AddScoped<FloorService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<BedService>();
builder.Services.AddScoped<AccomidationTypeService>();
builder.Services.AddScoped<ServicesCatalogueService>();
builder.Services.AddScoped<RoomAmenitiesService>();
builder.Services.AddScoped<OccupantGuestService>();
builder.Services.AddScoped<DepartmentMappingService>();
builder.Services.AddScoped<SystemParametersService>();
builder.Services.AddScoped<CorpAgreementService>();
builder.Services.AddScoped<GstConfigService>();
builder.Services.AddScoped<TariffService>();
builder.Services.AddScoped<CheckinService>();
builder.Services.AddScoped<CheckoutService>();
builder.Services.AddScoped<HkDashboardService>();
builder.Services.AddScoped<LookupService>();
builder.Services.AddScoped<ReservationsService>();
builder.Services.AddScoped<GuestFolioBillingService>();
builder.Services.AddScoped<ActiveGuestsService>();
builder.Services.AddScoped<RoomTransferService>();
builder.Services.AddScoped<CorporateInvoicesService>();
builder.Services.AddScoped<WaitingListService>();
builder.Services.AddScoped<LaundryLogService>();
builder.Services.AddScoped<DepositLedgerService>();
builder.Services.AddScoped<GstReportsService>();
builder.Services.AddScoped<MaintenanceRequestsService>();
builder.Services.AddScoped<RevenueReportsService>();
builder.Services.AddScoped<ExecutiveDashboardService>();
builder.Services.AddScoped<OccupancyReportsService>();
builder.Services.AddScoped<ReceptionDashboardService>();

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

// Default static files (wwwroot)
app.UseStaticFiles();

// Uploads folder static files
var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "Uploads");
Directory.CreateDirectory(uploadsPath);   // creates the folder if it does not exist

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/Uploads"
});

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