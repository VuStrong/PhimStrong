namespace PhimStrong.Models.Video
{
    public class VideoViewModel
    {
        public int Id { get; set; }
        public string MovieId { get; set; }
        public string? VideoUrl { get; set; }
        public int? Episode { get; set; }
        public int? Length { get; set; }

        public VideoViewModel(string movieId) 
        {
            MovieId = movieId;
        }
    }
}
