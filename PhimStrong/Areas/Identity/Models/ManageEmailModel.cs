using System.ComponentModel.DataAnnotations;

namespace PhimStrong.Areas.Identity.Models
{
    public class ManageEmailModel
    {
#nullable disable
        public bool IsEmailConfirmed { get; set; }

        public string Email { get; set; }

        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }
    }
}
