using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartLearning.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();

var connectionString = builder.Configuration.GetConnectionString("DevDB") ?? string.Empty;
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseMySQL(connectionString));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = 
        options.DefaultChallengeScheme = 
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:JWTSecret").Value!)),
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "api");
    });
}

app.UseHttpsRedirection();

app.UseCors(options =>
    options.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader());

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app
    .MapGroup("api/Auth")
    .MapIdentityApi<AppUser>();

app.Run();