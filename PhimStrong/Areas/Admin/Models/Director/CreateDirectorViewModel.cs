using System.ComponentModel.DataAnnotations;
#pragma warning disable

namespace PhimStrong.Areas.Admin.Models.Director
{
    public class CreateDirectorViewModel
    {
        [Required(ErrorMessage = "Chưa nhập tên.")]
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string? About { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public IFormFile? AvatarFile { get; set; }
        [FileExtensions(Extensions = ".png,.jpg,.jpeg")]
        public string? AvatarFileName
        {
            get
            {
                if (AvatarFile != null)
                    return AvatarFile.FileName;
                else
                    return null;
            }
        }
    }
}
