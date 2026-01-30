using cfs.demo.Data;
using cfs.demo.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore; // Ensure this is present
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using SQLitePCL;

Batteries.Init();

var builder = WebApplication.CreateBuilder(args);

// Ensure SQLite folder exists BEFORE DbContext is configured
var dbDir = "/home/data"; 
if (!Directory.Exists(dbDir)) 
{ 
    Directory.CreateDirectory(dbDir); 
}

var azureAdSection = builder.Configuration.GetSection("AzureAd");
var clientId = azureAdSection["ClientId"];
var audience = azureAdSection["Audience"];

// Register DbContext and database service
builder.Services.AddDbContext<CfsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CfsDatabase")));

builder.Services.AddScoped<ICfsDatabase, CfsDatabase>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}/v2.0";
        //options.Audience = builder.Configuration["AzureAd:ClientId"];
        // Disable audience validation entirely (accept any aud)
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();


// existing auth / other registration (commented out earlier) can be re-enabled if needed
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CfsDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
