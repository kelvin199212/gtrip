using Fillow.Models.partneradmin;
using Fillow.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddSingleton<ITravelPlanService,TravelPlanService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<IJourneyOrderService,JourneyOrderService>();
builder.Services.AddSingleton<IScheduleService, ScheduleService>();
builder.Services.AddSingleton<IUserProfileService, UserProfileService>();
builder.Services.AddSingleton<IVerificationService, VerificationService>();
builder.Services.AddSingleton<IWithdrawalService, WithdrawalService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});
// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAllOrigins", builder =>
	{
		builder.AllowAnyOrigin() // Allows all origins (useful for development; use more restrictive settings for production)
			   .AllowAnyMethod() // Allows all HTTP methods (GET, POST, PUT, DELETE, etc.)
			   .AllowAnyHeader(); // Allows all headers
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession(); // Enable session middleware

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Fillow}/{action=Index}/{id?}");
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
