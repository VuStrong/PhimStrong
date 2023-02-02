using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PhimStrong.Areas.Identity.Models;
using SharedLibrary.Models;
using System.Text.Encodings.Web;
using System.Text;
using System.Security.Claims;
using SharedLibrary.Constants;
using SharedLibrary.Helpers;

#pragma warning disable
namespace PhimStrong.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AuthenticationController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

		public AuthenticationController(SignInManager<User> signInManager, UserManager<User> userManager, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        [Route("/register")]
        public async Task<IActionResult> Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            return View(new RegisterModel() { ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList() });
        }

        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model, [FromRoute] string? returnUrl = null)
        {
            returnUrl ??= Url.Action("Index", "Home", new { area = "" });

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    ModelState.AddModelError(string.Empty, "Email đã tồn tại.");
                    return View(new RegisterModel() { ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList() });
                }

                user = new User()
                { 
                    UserName = model.Email,
                    Email = model.Email,
                    DisplayName = model.Name,
                    RoleName = RoleConstant.MEMBER
                };

                user.NormalizeDisplayName = user.DisplayName.RemoveMarks();
                
                var result = await _userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, RoleConstant.MEMBER);

                    string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    string callbackUrl = Url.Action("ConfirmEmail", "Authentication",
                            new
                            {
                                area = "Identity",
                                token = token,
                                userid = user.Id
                            },
                            protocol: Request.Scheme
                        );

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Xác thực Email",
                        $"Yô người mới !, click vào <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>đây</a> để xác thực Email của bạn nhé :)"
                    );

                    await _signInManager.SignInAsync(user, false);
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(new RegisterModel() { ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList() });
        }

        [HttpGet]
        [Route("/login")]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            return View(new LoginModel() { ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList() });
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login([FromForm] LoginModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Action("Index", "Home", new { area = "" });

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return LocalRedirect(returnUrl);
                    }
                    if (result.IsLockedOut)
                    {
                        return RedirectToAction("Lockout");
                    }
                }

                ModelState.AddModelError(string.Empty, "Email hoặc mật khấu không chính xác.");
            }

            return View(new LoginModel() { ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList() });
        }

        [HttpGet]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userid, string token)
        {
            if (userid == null || token == null)
            {
                return View(model: "Lỗi xác thực Email.");
            }

            var user = await _userManager.FindByIdAsync(userid);

            if (user == null)
            {
                return View(model: "Lỗi xác thực Email.");
            }

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return View(model: "Lỗi xác thực Email.");
            }

            return View(model: "Xác thực Email thành công. Chúc xem phim vui vẻ nhé :>");
        }

        [HttpGet]
        public IActionResult AccessDenied(string text = null)
        {
            return View(model:text);
        }

        [HttpGet]
        [Route("/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("/forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Action(
                    "ResetPassword", 
                    "Authentication",
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Reset Password",
                    $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Ấn vào đây</a> để đặt lại mật khẩu của bạn.");

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View();
        }

        [HttpGet]
        [Route("/forgot-password-confirmation")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [Route("/reset-password")]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest("Cần có mã xác thực để đặt lại mật khẩu.");
            }
            else
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                
                return View(model);
            }
        }

        [HttpPost]
        [Route("/reset-password")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [Route("/reset-password-confirmation")]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ExternalLogin() => RedirectToAction("Login");


        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Action("Index", "Home", new { area = "" });

            if (remoteError != null)
            {
                TempData["status"] = $"Error from external provider: {remoteError}";
                return RedirectToAction("Login");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["status"] = "Error loading external login information.";
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
				return RedirectToAction("Lockout");
			}
			else
            {
                var user = new User();
                user.EmailConfirmed = true;

                // get profile's picture from google account :
                if (info.Principal.HasClaim(c => c.Type == "image"))
                {
                    user.Avatar = info.Principal.FindFirstValue("image");
                }

                // get profile's name from google account :
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                {
                    user.DisplayName = info.Principal.FindFirstValue(ClaimTypes.Name);
                    user.NormalizeDisplayName = user.DisplayName.RemoveMarks();
                }

                // get Email address from google account :
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    user.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    user.UserName = user.Email;
                }

                var result2 = await _userManager.CreateAsync(user);
                if (result2.Succeeded)
                {
                    result2 = await _userManager.AddLoginAsync(user, info);

                    if (result2.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                else
                {
                    TempData["status"] = "Lỗi, Email đã tồn tại !";
				}

                return RedirectToAction("Login");
            }
        }
    }
}
