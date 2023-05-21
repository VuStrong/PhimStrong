using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PhimStrong.Areas.Identity.Models;
using System.Text.Encodings.Web;
using System.Text;
using PhimStrong.Application.Interfaces;

#pragma warning disable
namespace PhimStrong.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AuthenticationController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEmailSender _emailSender;

        public AuthenticationController(
            IUserService userService,
            IAuthenticationService authenticationService, 
            IEmailSender emailSender)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _emailSender = emailSender;
        }

        [HttpGet("/register")]
        public async Task<IActionResult> Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            return View(new RegisterModel() 
            {
                ExternalLogins = (await _authenticationService.GetExternalAuthenticationSchemesAsync()).ToList() 
            });
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model, [FromRoute] string? returnUrl = null)
        {
            returnUrl ??= Url.Action("Index", "Home", new { area = "" });

            if (ModelState.IsValid)
            {
                var result = await _authenticationService.RegisterAsync(model.Name, model.Email, model.Password);

                if (result.Success)
                {
                    string token = await _userService.GenerateEmailConfirmationTokenAsync(result.User);
                    token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    string callbackUrl = Url.Action("ConfirmEmail", "Authentication",
                            new
                            {
                                area = "Identity",
                                token = token,
                                userid = result.User.Id
                            },
                            protocol: Request.Scheme
                        );

                    await _emailSender.SendEmailAsync(
						result.User.Email,
                        "Xác thực Email",
                        $"Yô người mới !, click vào <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>đây</a> để xác thực Email của bạn nhé :)"
                    );

					return LocalRedirect(returnUrl);
				}
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
			}

            return View(new RegisterModel() 
            {
                ExternalLogins = (await _authenticationService.GetExternalAuthenticationSchemesAsync()).ToList() 
            });
        }

        [HttpGet("/login")]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            return View(new LoginModel()
            {
                ExternalLogins = (await _authenticationService.GetExternalAuthenticationSchemesAsync()).ToList()
            });
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromForm] LoginModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Action("Index", "Home", new { area = "" });

            if (ModelState.IsValid)
            {
                var result = await _authenticationService.LoginAsync(model.Email, model.Password, model.RememberMe);

				if (result.Success)
				{
					return LocalRedirect(returnUrl);
				}
				if (result.IsLockedOut)
				{
					return RedirectToAction("Lockout");
				}

				ModelState.AddModelError(string.Empty, "Email hoặc mật khấu không chính xác.");
            }

            return View(new LoginModel()
            {
                ExternalLogins = (await _authenticationService.GetExternalAuthenticationSchemesAsync()).ToList() 
            });
        }

        [HttpGet]
        public IActionResult Lockout() => View();

        [HttpPost]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            await _authenticationService.LogoutAsync();

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

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userService.ConfirmEmailAsync(userid, token);

            if (!result.Success)
            {
                return View(model: "Lỗi xác thực Email.");
            }

            return View(model: "Xác thực Email thành công. Chúc xem phim vui vẻ nhé :>");
        }

        [HttpGet]
        public IActionResult AccessDenied(string text = null) => View(model: text);

        [HttpGet("/forgot-password")]
        public IActionResult ForgotPassword() => View();

        [HttpPost("/forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var code = await _userService.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Authentication",
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                _emailSender.SendEmailAsync(
                    model.Email,
                    "Reset Password",
                    $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Ấn vào đây</a> để đặt lại mật khẩu của bạn.");

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View();
        }

        [HttpGet("/forgot-password-confirmation")]
        public IActionResult ForgotPasswordConfirmation() => View();

        [HttpGet("/reset-password")]
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

        [HttpPost("/reset-password")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.ResetPasswordAsync(model.Email, model.Code, model.Password);
            if (result.Success)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

			foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        [HttpGet("/reset-password-confirmation")]
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
            var properties = _authenticationService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
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

            var result = await _authenticationService.ExternalLoginAsync();

            if (result.Success)
            {
                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                return RedirectToAction("Lockout");
            }

            if (result.Errors != null && result.Errors.Count > 0) 
                TempData["status"] = result.Errors[0];

            return RedirectToAction("Login");
        }
    }
}
