using EventPlannerWeb.Data;
using EventPlannerWeb.Interfaces;
using EventPlannerWeb.Models;
using EventPlannerWeb.Services;
using Library_kursova.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContextFactory<EventPlannerContext>(opt =>
{
    opt.UseNpgsql("Host=database-1.c5cwsas8ab06.us-east-1.rds.amazonaws.com;Port=5432;Username=postgres;Password=vitalik31068;Database=EventPlanner");
});

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddIdentityCore<User>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.User.RequireUniqueEmail = true;
})
    .AddRoles<AppRole>()
    .AddRoleManager<RoleManager<AppRole>>()
    .AddEntityFrameworkStores<EventPlannerContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding
                .UTF8.GetBytes(builder.Configuration["TokenKey"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", context =>
    {
        context.Response.Redirect("/api/guest");
        return Task.CompletedTask;
    });
});

//app.UseAuthentication();

app.MapControllers();

app.Run();
