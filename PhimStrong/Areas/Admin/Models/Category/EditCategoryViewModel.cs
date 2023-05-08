using System.ComponentModel.DataAnnotations;
#pragma warning disable

namespace PhimStrong.Areas.Admin.Models.Category
{
    public class EditCategoryViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Chưa nhập tên.")]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
