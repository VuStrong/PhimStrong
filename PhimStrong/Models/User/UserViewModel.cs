using PhimStrong.Models.Movie;

namespace PhimStrong.Models.User
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? Avatar { get; set; }
        public string? FavoriteMovie { get; set; }
        public string? Hobby { get; set; }
        public string? RoleName { get; set; }
        public List<MovieViewModel>? LikedMovies { get; set; }

        public UserViewModel(string id, string displayName, string phoneNumber, string email)
        {
            Id = id;
            DisplayName = displayName;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}
