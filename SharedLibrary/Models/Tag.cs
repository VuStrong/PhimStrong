using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.Models
{
#pragma warning disable
	public class Tag
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public virtual Movie Movie { get; set; }

		public string TagName { get; set; }
	}
}
