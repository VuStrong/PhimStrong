using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace PhimStrong.Areas.Identity.Models
{
    public class RegisterModel
    {
#pragma warning disable
        [Required]
        [Display(Name = "Tên người dùng")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} phải có độ dài tối thiểu {2} kí tự và tối đa {1} kí tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        public List<AuthenticationScheme>? ExternalLogins { get; set; }

    }
}
