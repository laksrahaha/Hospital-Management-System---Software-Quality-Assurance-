//sets up and runs the reserve health api
// this is incduing the database connection , contorller, CORS and sample apteint data
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReserveHealth.Api.Data;
using ReserveHealth.Api.Models; 
using ReserveHealth.Api.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Provides secure password hashing and verification for staff accounts.
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Generates JWT tokens after successful staff login.
builder.Services.AddScoped<TokenGenerate>();

// Configures JWT authentication so protected API endpoints
// only accept requests with a valid token.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                )
            )
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ReserveHealthContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ReserveHealthContext>();

    SeedData.AddSamplePatients(context);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Frontend");

// Checks who the user is before access rules are evaluated.
app.UseAuthentication();

// Checks whether the authenticated user has permission
// to access the requested endpoint.
app.UseAuthorization();

app.MapControllers();

app.Run();