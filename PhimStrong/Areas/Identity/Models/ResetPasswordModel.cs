using System.ComponentModel.DataAnnotations;

namespace PhimStrong.Areas.Identity.Models
{
    public class ResetPasswordModel
    {
#pragma warning disable
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} phải có độ dài tối thiểu {2} kí tự và tối đa {1} kí tự", MinimumLength = 6)]
        [Display(Name = "Mật khẩu mới")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
