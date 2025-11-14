using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using S1.Core.Entities;
using System.Security.Claims;

namespace S1.Core.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public AuthorizationController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest()
                          ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // Get user from Identity cookie
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

            if (!result.Succeeded || result.Principal == null)
            {
                // Redirect to login if not login
                return Challenge(
                    authenticationSchemes: IdentityConstants.ApplicationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                            Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                    });
            }

            var user = result.Principal;

            // Create claims for OpenIddict token
            var claims = new List<Claim>
            {
                new Claim(OpenIddictConstants.Claims.Subject, user.FindFirstValue(ClaimTypes.NameIdentifier)!),
                new Claim(OpenIddictConstants.Claims.Name, user.Identity?.Name ?? ""),
                new Claim(OpenIddictConstants.Claims.Email, user.FindFirstValue(ClaimTypes.Email) ?? "")
            };

            // Get roles from Identity user
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var userEntity = await _userManager.FindByIdAsync(userId);
                if (userEntity != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(userEntity);
                    foreach (var role in userRoles ?? Enumerable.Empty<string>())
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }
            }

            // Create identity + principal
            var identity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Assign scope by request
            principal.SetScopes(request.GetScopes());
            principal.SetResources("api");

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest()
                          ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            ClaimsPrincipal principal;

            if (request.IsClientCredentialsGrantType())
            {
                var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());
                principal = new ClaimsPrincipal(identity);
                principal.SetScopes(request.GetScopes());
            }
            else if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
            {
                principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal!;
            }
            else
            {
                throw new InvalidOperationException("The specified grant type is not supported.");
            }

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo")]
        public async Task<IActionResult> Userinfo()
        {
            var principal = (await HttpContext.AuthenticateAsync(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

            if (principal == null)
                return Unauthorized();

            var roles = principal.FindAll("role")
                         .Select(c => c.Value)
                         .ToArray();

            return Ok(new
            {
                sub = principal.FindFirstValue(OpenIddictConstants.Claims.Subject),
                name = principal.FindFirstValue(OpenIddictConstants.Claims.Name),
                email = principal.FindFirstValue(OpenIddictConstants.Claims.Email),
                role = roles
            });
        }
    }
}
