using MES.Shared.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using S1.Core.Configurations;
using S1.Core.Entities;
using S1.Core.Enums;
using S1.Core.Models;

namespace S1.Core.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ILogger<AccountController> _logger;
        private readonly CookieSettings _cookieSettings;

        public AccountController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IStringLocalizer<SharedResources> localizer,
            ILogger<AccountController> logger,
            IOptions<CookieSettings> cookieSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _localizer = localizer;
            _logger = logger;
            _cookieSettings = cookieSettings.Value;
        }

        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError(string.Empty, _localizer["Login_Failed"]);
                return View(model);
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError(string.Empty, _localizer["Login_Locked_Out"]);
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, _localizer["Login_Not_Allowed"]);
                return View(model);
            }

            if (!await _userManager.IsInRoleAsync(user, AppRole.Admin.ToString())
                && !await _userManager.IsInRoleAsync(user, AppRole.User.ToString()))
            {
                ModelState.AddModelError(string.Empty, _localizer["Login_Role_Not_Allowed"]);
                return View(model);
            }

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(_cookieSettings.RememberMeExpireDays)
                    : DateTimeOffset.UtcNow.AddSeconds(_cookieSettings.DefaultExpireSeconds)
            };

            await _signInManager.SignInAsync(user, authProperties);

            if (!string.IsNullOrEmpty(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
