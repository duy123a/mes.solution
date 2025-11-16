using MES.Shared;
using MES.Shared.Utilities;
using OpenIddict.Abstractions;
using S1.Core.Data.Context;
using S1.Core.Services;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
var adminEmail = builder.Configuration["SeedUsers:AdminEmail"] ?? string.Empty;
var adminPassword = builder.Configuration["SeedUsers:AdminPassword"] ?? string.Empty;

// config origin
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ---------- Identity ----------
builder.Services.AddInfrastructureServices(builder.Configuration);

// ---------- OpenIddict ----------
builder.Services.AddOpenIddict()
    .AddCore(opt =>
    {
        opt.UseEntityFrameworkCore()
           .UseDbContext<AuthDbContext>();
    })
    .AddServer(opt =>
    {
        // Endpoints
        opt.SetAuthorizationEndpointUris("/connect/authorize")
           .SetTokenEndpointUris("/connect/token")
           .SetRevocationEndpointUris("/connect/revoke")
           .SetUserInfoEndpointUris("/connect/userinfo")
           .SetEndSessionEndpointUris("connect/logout");

        // Flows
        opt.AllowAuthorizationCodeFlow()
           .RequireProofKeyForCodeExchange();
        opt.AllowPasswordFlow()
           .AllowRefreshTokenFlow();

        // Scopes
        opt.RegisterScopes(
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Roles
        );

        // Keys
        // Dev: ephemeral keys, Production: replace with certificate
        opt.AddEphemeralEncryptionKey()
           .AddEphemeralSigningKey();

        opt.DisableAccessTokenEncryption();

        // ASP.NET Core integration
        opt.UseAspNetCore()
           .EnableAuthorizationEndpointPassthrough()
           .EnableTokenEndpointPassthrough()
           .EnableUserInfoEndpointPassthrough()
           .EnableEndSessionEndpointPassthrough();
    })
    .AddValidation(opt =>
    {
        opt.UseLocalServer();
        opt.UseAspNetCore();
    });

// ---------- Seed demo clients ----------
builder.Services.AddHostedService<DatabaseSeedWorker>();

// ---------- MVC ----------
builder.Services.AddAppLocalization();
builder.Services.AddControllersWithViews(o => o.AddStringTrimModelBinderProvider());

var app = builder.Build();

// ---------- Pipeline ----------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    await SeedManager.SeedRolesAsync(scope.ServiceProvider);
    await SeedManager.SeedAdminAccountAsync(scope.ServiceProvider, adminEmail, adminPassword);
}

app.UseRequestLocalization();
app.UseCors();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

if (app.Environment.IsDevelopment())
{
    app.UseWebSockets();
}

app.Run();
