using System.ComponentModel.DataAnnotations;
#pragma warning disable

namespace PhimStrong.Areas.Admin.Models.Country
{
    public class EditCountryViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? About { get; set; }
    }
}
