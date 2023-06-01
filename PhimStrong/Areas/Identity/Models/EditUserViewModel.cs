namespace PhimStrong.Areas.Identity.Models
{
    public class EditUserViewModel
    {
        public string? DisplayName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FavoriteMovie { get; set; }
        public string? Hobby { get; set; }
        public string? Avatar { get; set; }
        public IFormFile? AvatarFile { get; set; }
    }
}
