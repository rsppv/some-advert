using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SomeAdvert.Web.Models.Accounts;

namespace SomeAdvert.Web.Controllers
{
    [AllowAnonymous]
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly CognitoUserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _userPool;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(
            SignInManager<CognitoUser> signInManager,
            UserManager<CognitoUser> userManager,
            CognitoUserPool userPool,
            ILogger<AccountsController> logger)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userManager = (userManager as CognitoUserManager<CognitoUser>) ??
                           throw new ArgumentNullException(nameof(userManager));
            _userPool = userPool ?? throw new ArgumentNullException(nameof(userPool));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Signup()
        {
            return await Task.FromResult<IActionResult>(View(new SignupViewModel()));
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userPool.GetUser(model.Email);
                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation(@$"User account successfully created for {model.Email}");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Confirm");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Confirm()
        {
            return await Task.FromResult<IActionResult>(View(new ConfirmAccountViewModel()));
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(userEmail))
                {
                    ModelState.AddModelError("NoAuthenticatedUser", "There is no authenticated user");
                    return View(model);
                }

                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return NotFound($"Unable to find user with email '{userEmail}'");
                }

                var result = await _userManager.ConfirmSignUpAsync(user, model.Code, forcedAliasCreation: true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("LoginFailed",
                        "Login failed. Make sure provided email and password are correct");
                    return View(model);
                }
                _logger.LogInformation("User logged in");
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }
        
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) 
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return RedirectToAction("ResetPassword");
            }

            await user.ForgotPasswordAsync();
            return RedirectToAction("ResetPassword", new ForgotPasswordViewModel{ Email = model.Email });
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> PostResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ResetPassword", model);
            }

            var user = await _userManager.FindByIdAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("UserNotFound", "Unable to find user");
                return View("ResetPassword", model);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.ResetToken, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation($@"Password successfully reset for user '{model.Email}'");
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            
            return View("ResetPassword", model);
        }
    }
}