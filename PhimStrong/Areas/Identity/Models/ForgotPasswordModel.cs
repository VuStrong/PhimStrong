using System.ComponentModel.DataAnnotations;

namespace PhimStrong.Areas.Identity.Models
{
    public class ForgotPasswordModel
    {
#nullable disable
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
