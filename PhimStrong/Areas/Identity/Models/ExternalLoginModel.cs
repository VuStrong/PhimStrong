using System.ComponentModel.DataAnnotations;

namespace PhimStrong.Areas.Identity.Models
{
    public class ExternalLoginModel
    {
#nullable disable
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string ProviderDisplayName { get; set; }
    }
}
