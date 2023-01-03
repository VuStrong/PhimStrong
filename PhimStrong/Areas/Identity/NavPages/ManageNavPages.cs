
namespace PhimStrong.Areas.Identity.NavPages
{
    public class ManageNavPages
    {
        public static string IndexNavClass(string page)
        {
            string value = page == "Index" ? "active" : "";
            return value;
        }

        public static string ChangePasswordNavClass(string page)
        {
            string value = page == "ChangePassword" ? "active" : "";
            return value;
        }
    }
}
