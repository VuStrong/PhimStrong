using System.ComponentModel.DataAnnotations;

namespace PhimStrong.Domain.Models
{
#pragma warning disable
	public class Tag
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public Movie Movie { get; set; }

		public string TagName { get; set; }
	}
}
