using PhimStrong.Models.Category;
using PhimStrong.Models.Country;
using PhimStrong.Models.User;

namespace PhimStrong.Models
{
	public class HeaderNavBarViewModel
	{
#pragma warning disable
		public UserViewModel? User { get; set; }
		public List<CategoryViewModel> Categories { get; set; }
		public List<CountryViewModel> Countries { get; set; }
		public List<int> Years { get; set; }
	}
}
