using System.ComponentModel.DataAnnotations;

namespace PhimStrong.Areas.Identity.Models
{
	public class ChangePasswordModel
	{
#nullable disable
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Mật khẩu cũ")]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(50, ErrorMessage = "{0} phải có độ dài tối thiểu {2} kí tự và tối đa {1} kí tự", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Mật khẩu mới")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Nhập lại mật khẩu mới")]
		[Compare("NewPassword", ErrorMessage = "Mật khẩu không khớp.")]
		public string ConfirmPassword { get; set; }
	}
}
