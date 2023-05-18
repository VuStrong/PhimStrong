using Microsoft.AspNetCore.Identity;

namespace PhimStrong.Domain.Models
{
	// Put IdentityUser here because I don't know where else to put it :((
	public class User : IdentityUser
    {
        public User() : base() {}

        public string? DisplayName { get; set; }
        public string? NormalizeDisplayName { get; set; }
        public string? Avatar { get; set; }
        public string? FavoriteMovie { get; set; }
        public string? Hobby { get; set; }
        public string? RoleName { get; set; }
        public List<Movie>? LikedMovies { get; set; }
    }
}
