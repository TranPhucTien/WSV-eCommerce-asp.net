using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using shopDev.Configurations;
using shopDev.Middlewares;
using shopDev.Services;
using shopDev.Utils.Errors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ErrorHandlingMiddleware>();
builder.Services.AddTransient<CheckApiKeysMiddleware>();
builder.Services.AddTransient<CheckPermissionsMiddleware>();
builder.Services.AddTransient<AuthenticationMiddleware>();
builder.Services.AddSingleton<ProblemDetailsFactory, ShopDevProblemDetailsFactory>();

// Database
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("MongoDatabase"));

// Services
builder.Services.AddSingleton<ShopsService>();
builder.Services.AddSingleton<AccessService>();
builder.Services.AddSingleton<KeyTokensService>();
builder.Services.AddSingleton<ApiKeyService>();

// Authentication
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    var secretKeyBytes = Encoding.UTF8.GetBytes("2Y7aOxlnCFgUXLL4ja41GdbC1lfvn1Ay");
    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,

        ValidateIssuerSigningKey = false,
        // IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes)
        ValidateLifetime = true
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler("/error");

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<CheckApiKeysMiddleware>();
app.UseMiddleware<CheckPermissionsMiddleware>();
// app.UseMiddleware<AuthenticationMiddleware>();

app.UseWhen(
    context => context.Request.Path.ToString().Contains("/api/shop/logout") || 
               context.Request.Path.ToString().Contains("/api/shop/refresh-token"),
    branch=> branch.UseMiddleware<AuthenticationMiddleware>());

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();