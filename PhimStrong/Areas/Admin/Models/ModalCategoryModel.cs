using SharedLibrary.Models;

namespace PhimStrong.Areas.Admin.Models
{
    public class ModalCategoryModel
    {
        public List<string>? SelectedCategories { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
