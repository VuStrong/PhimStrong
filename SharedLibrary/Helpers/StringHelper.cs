using System.Text.RegularExpressions;

namespace SharedLibrary.Helpers
{
    public static class StringHelper
    {
        public static string NormalizeString(this string value)
        {
            return Regex.Replace(value.ToLower().Trim(), @"(^\w)|(\s\w)", m => m.Value.ToUpper());
        }

        public static string RemoveMarks(this string value)
        {
            string output = value;
            output = output.ToLower().Trim();

            output = Regex.Replace(output, @"é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ", m => "e");
            output = Regex.Replace(output, @"ý|ỳ|ỷ|ỹ|ỵ", m => "y");
            output = Regex.Replace(output, @"ú|ù|ủ|ũ|ụ|ứ|ừ|ử|ữ|ự|ư", m => "u");
            output = Regex.Replace(output, @"í|ì|ỉ|ĩ|ị", m => "i");
            output = Regex.Replace(output, @"ó|ò|ỏ|õ|ọ|ơ|ớ|ờ|ở|ỡ|ợ|ô|ố|ồ|ổ|ỗ|ộ", m => "o");
            output = Regex.Replace(output, @"à|á|ả|ã|ạ|â|ấ|ầ|ẩ|ẫ|ậ|ă|ắ|ằ|ẵ|ặ|ẳ", m => "a");
            output = Regex.Replace(output, @"đ", m => "d");

            return output;
        }
    }
}
