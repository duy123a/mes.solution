using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Authentication + OpenID Connect
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "S1"; // using OpenID Connect challenge
})
.AddCookie()
.AddOpenIdConnect("S1", options =>
{
    options.Authority = "https://localhost:5001";
    options.ClientId = "S2";
    options.ClientSecret = "S2Secret";
    options.ResponseType = "code";
    options.SaveTokens = true;

    options.Scope.Clear();
    options.Scope.Add("api");
    options.Scope.Add("openid");
    options.Scope.Add("email");
    options.Scope.Add("profile");
    options.Scope.Add("roles");

    options.GetClaimsFromUserInfoEndpoint = true;

    // Map role from openid scope to ClaimsPrincipal
    options.ClaimActions.MapJsonKey(ClaimTypes.Role, ClaimTypes.Role);

    options.TokenValidationParameters.NameClaimType = "name";
    options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
