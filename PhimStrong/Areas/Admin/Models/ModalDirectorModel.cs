using SharedLibrary.Models;

namespace PhimStrong.Areas.Admin.Models
{
    public class ModalDirectorModel
    {
        public List<string>? SelectedDirectors { get; set; }
        public List<Director>? Directors { get; set; }
    }
}
