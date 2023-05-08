using System.ComponentModel.DataAnnotations;
#pragma warning disable

namespace PhimStrong.Areas.Admin.Models.Country
{
    public class CreateCountryViewModel
    {
        [Required]
        public string Name { get; set; }
        public string? About { get; set; }
    }
}
