using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
//using Npgsql.Json.NET;
//using SU.PMJAY.Security.Api.Repositories;
//using SU.PMJAY.Security.Api.Services;
//using Suvarna.PM.NX.Api.Repositories;
//using Suvarna.PM.NX.Api.Services;
using Suvarna.PMJAY.NHCX.Api.Data;
//using Suvarna.PMJAY.NHCX.Api.Fhir;
//using Suvarna.PMJAY.NHCX.Api.Filters;
//using Suvarna.PMJAY.NHCX.Api.Interface;
//using Suvarna.PMJAY.NHCX.Api.Middleware;
//using Suvarna.PMJAY.NHCX.Api.Models;
//using Suvarna.PMJAY.NHCX.Api.Repositories;
//using Suvarna.PMJAY.NHCX.Api.Repository;
//using Suvarna.PMJAY.NHCX.Api.Services;
//using Suvarna.PMJAY.Security.Repositories;
//using Suvarna.PMJAY.Security.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// 🔥 ENABLE NEWTONSOFT JSON FOR NPGSQL (CRITICAL)
NpgsqlConnection.GlobalTypeMapper.UseJsonNet();
#region CORS
// =====================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("*");
    });
});
#endregion

#region Swagger
// =====================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Suvarna PMJAY NHCX API",
        Version = "v1"
    });

    // 🔐 JWT support in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

#region Database & Repositories
// =====================================================
builder.Services.AddSingleton<DbHelper>();

//builder.Services.AddScoped<UserRepository>();
//builder.Services.AddScoped<SessionRepository>();
//builder.Services.AddScoped<AuditRepository>();
//builder.Services.AddScoped<ErrorRepository>();
//builder.Services.AddScoped<TotpRepository>();
//builder.Services.AddScoped<OTPRepository>();
//builder.Services.AddScoped<UserOtpRepository>();
//builder.Services.AddScoped<ForgotPasswordRepository>();
//builder.Services.AddScoped<HospitalRepository>();
//builder.Services.AddScoped<LocationRepository>();
//builder.Services.AddScoped<RoleRepository>();
//builder.Services.AddScoped<UserRoleMappingRepository>();
//builder.Services.AddScoped<UserCreationRepository>();
//builder.Services.AddScoped<ChangePasswordRepository>();
//builder.Services.AddScoped<DocumentHeaderRepository>();
//builder.Services.AddScoped<DocumentRepository>();
//builder.Services.AddScoped<DefaultDocumentRepository>();
//builder.Services.AddScoped<RoleDocumentMapRepository>();
//builder.Services.AddScoped<RoleDocumentMappingRepository>();
//builder.Services.AddScoped<SidebarRepository>();
//builder.Services.AddScoped<SpecialtyRepository>();
//builder.Services.AddScoped<DoctorRepository>();
//builder.Services.AddScoped<PackageRepository>();
//builder.Services.AddScoped<SpecialtyTypeRepository>();
//builder.Services.AddScoped<PatientRegistrationRepository>();
//builder.Services.AddScoped<IClaimResponseParser, ClaimResponseParser>();
//builder.Services.AddScoped<PreauthRepository>();
//builder.Services.AddScoped<CoverageEligibilityService>();
//builder.Services.AddScoped<InsurancePlanService>();
//builder.Services.AddScoped<PreauthFhirBuilder_ExactPinToPin>();
//builder.Services.AddScoped<PmjayCallbackRepository>();
//builder.Services.AddScoped<DashBoardRepository>();
//builder.Services.AddScoped<ClaimPatientRepository>();
//builder.Services.AddScoped<HimsPatientRepository, HimsPatientRepository>();
//builder.Services.AddScoped<HimsPatientService, HimsPatientService>();
//builder.Services.AddScoped<PatientCaseRepository>();
//builder.Services.AddScoped<PatientCaseService>();
//builder.Services.AddScoped<PackageProcedureRepository>();
//builder.Services.AddScoped<PackageProcedureService>();
//builder.Services.AddScoped<ImplantRepository>();
//builder.Services.AddScoped<ImplantService>();
//builder.Services.AddScoped<StratificationGroupServices>();
//builder.Services.AddScoped<StratificationGroupRepository>();
//builder.Services.AddScoped<ProcedureCoverageService>();
//builder.Services.AddScoped<PmNxInsurancePlanRequestRepository>();
//builder.Services.AddScoped<PmNxInsurancePlanRequestService>();

#endregion

#region Services
// =====================================================
//builder.Services.AddScoped<LoginService>();
//builder.Services.AddScoped<NotificationService>();
//builder.Services.AddScoped<TotpService>();
//builder.Services.AddScoped<UserOtpService>();
//builder.Services.AddScoped<ForgotPasswordService>();
//builder.Services.AddScoped<JwtService>();
//builder.Services.AddScoped<HospitalService>();
//builder.Services.AddScoped<LocationService>();
//builder.Services.AddScoped<RoleService>();
//builder.Services.AddScoped<UserRoleMappingService>();
//builder.Services.AddScoped<UserCReationService>();
//builder.Services.AddScoped<ChangePasswordService>();
//builder.Services.AddScoped<DocumentHeaderService>();
//builder.Services.AddScoped<DocumentService>();
//builder.Services.AddScoped<DefaultDocumentService>();
//builder.Services.AddScoped<RoleDocumentMapService>();
//builder.Services.AddScoped<RoleDocumentMappingService>();
//builder.Services.AddScoped<SidebarService>();
//builder.Services.AddScoped<SpecialtyService>();
//builder.Services.AddScoped<DoctorService>();
//builder.Services.AddScoped<PackageService>();
//builder.Services.AddScoped<SpecialtyTypeService>();
//builder.Services.AddScoped<PatientRegistrationService>();
//builder.Services.AddScoped<PreauthService>();
//builder.Services.AddScoped<PmjayCallbackService>();
//builder.Services.AddScoped<DashboardCountsServicess>();
//builder.Services.AddScoped<ClaimPatientService>();
//builder.Services.AddScoped<ClaimFhirBundel>();
//builder.Services.AddScoped<EnhancementFhirBundleBuilder>();
//builder.Services.AddScoped<PreauthRepository>();

//builder.Services.AddScoped<EmailService>();
//builder.Services.AddScoped<SmsService>();
//builder.Services.AddHttpClient<WhatsAppService>();
//builder.Services.AddHttpClient<PolicyFetchService>();
builder.Services.AddHttpClient(); // required for HttpClient injections
// If accessing HttpContext/User
builder.Services.AddHttpContextAccessor();
#endregion

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100MB
});


#region Controllers & Validation
// =====================================================
//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add<ValidateModelAttribute>();
//});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
#endregion

#region JWT Authentication
// =====================================================
//var jwtSection = builder.Configuration.GetSection("Jwt");
//builder.Services.Configure<JwtSettings>(jwtSection);

//var jwtSettings = jwtSection.Get<JwtSettings>()
//    ?? throw new Exception("JWT settings missing");

//var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//.AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
//    options.SaveToken = true;

//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtSettings.Issuer,
//        ValidAudience = jwtSettings.Audience,
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ClockSkew = TimeSpan.Zero
//    };

//    options.Events = new CustomAuthFailureHandler();
//});
#endregion

var app = builder.Build();

#region Middleware Pipeline (CORRECT ORDER)
// =====================================================

// 1️⃣ Global Exception Handler
//app.UseMiddleware<GlobalExceptionMiddleware>();

//// 2️⃣ Request Logging
//app.UseMiddleware<RequestLoggingMiddleware>();

// 3️⃣ HTTPS

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();

    app.UseHsts();
}

// 4️⃣ CORS
app.UseCors("AllowAll");

// 5️⃣ Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 6️⃣ Swagger (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 7️⃣ Controllers
app.MapControllers();

#endregion

app.Run();
