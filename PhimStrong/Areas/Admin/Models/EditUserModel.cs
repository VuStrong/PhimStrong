using PhimStrong.Models.User;

namespace PhimStrong.Areas.Admin.Models
{
	public class EditUserModel
	{
#nullable disable
		public List<string> RoleList { get; set; }
		public string UserRole { get; set; }
		public bool IsLock { get; set; }
		public UserViewModel User { get; set; }
	}
}
