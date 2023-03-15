using SharedLibrary.Models;

namespace PhimStrong.Areas.Admin.Models
{
    public class ModalCastModel
    {
        public List<string>? SelectedCasts { get; set; }
        public List<Cast>? Casts { get; set; }
    }
}
