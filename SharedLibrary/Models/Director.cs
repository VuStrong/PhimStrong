using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SharedLibrary.Models
{
    public class Director
    {
#pragma warning disable
        public string Id { get; set; }
        public int IdNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
		public string? NormalizeName { get; set; }

		[NotMapped]
        public IFormFile? AvatarFile { get; set; }
        [NotMapped]
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


        public string? Avatar { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [NotMapped]
        public int Age
        {
            get
            {
                return int.Parse(Math.Floor((double)(DateTime.Now - (this.DateOfBirth ?? DateTime.Now)).Days / 365).ToString());
            }
        }

        public virtual List<Movie>? Movies { get; set; }

        public string? About { get; set; }
    }
}
