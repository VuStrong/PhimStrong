using Microsoft.AspNetCore.Mvc.Rendering;
using PhimStrong.Models.Category;
using PhimStrong.Models.Country;

namespace PhimStrong.Models
{
	public class FilterViewModel
    {
#pragma warning disable
		public List<SelectListItem> Categories { get; set; }
		public SelectList Countries { get; set; }
		public SelectList Years { get; set; }
		public SelectList OrderByOptions { get; set; }
		public SelectList Types { get; set; }
	}
}
