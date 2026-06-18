using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Application.Interface;
using Infrastructure.Repository;
using Application.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Auth;
using Infrastructure.Context_model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// DbContexts
builder.Services.AddDbContext<AthleteDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContext<SubscriptionDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContext<SubscriptionPlanDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContext<LessonDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContext<ReservationDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContext<LocationDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContext<RoomDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContext<EquipmentSpotDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddDbContext<PaymentDbContext>(o => o.UseSqlServer(connectionString));

builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<AthleteDbContext>());

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Repositories
builder.Services.AddScoped<IAthleteRepository, AthleteRepository>();
builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IEquipmentRoomRepository, EquipmentRoomRepository>();
builder.Services.AddScoped<IEquipmentSpotRepository, EquipmentSpotRepository>();

// Infrastructure
builder.Services.AddScoped<IPaymentProcessor, PaymentProcessor>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordCrypt, PasswordCrypt>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Services
builder.Services.AddScoped<AthleteService>();
builder.Services.AddScoped<SubscriptionService>();
builder.Services.AddScoped<SubscriptionPlanService>();
builder.Services.AddScoped<WorkoutService>();
builder.Services.AddScoped<LessonService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<LocationService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<EquipmentService>();
builder.Services.AddScoped<EquipmentRoomService>();

builder.Services.AddHostedService<ScheduledJobsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
