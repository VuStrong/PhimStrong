using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace PhimStrong.Areas.Identity.Models
{
    public class LoginModel
    {
#pragma warning disable
        [Required(ErrorMessage = "Chưa nhập Email :()")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Chưa nhập mật khẩu :()")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public List<AuthenticationScheme>? ExternalLogins { get; set; }
    }
}
