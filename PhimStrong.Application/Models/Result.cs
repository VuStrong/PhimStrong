using Microsoft.AspNetCore.Identity;

namespace PhimStrong.Application.Models
{
    public class Result
    {
        public bool Success { get; set; }
        public List<string>? Errors { get; set; }

        public static Result OK()
        {
            return new Result { Success = true };
        }

        public static Result Error(IEnumerable<string> errors)
        {
            return new Result 
            {
                Success = false,
                Errors = errors.ToList()
            };
        }

        public static Result ToAppResult(IdentityResult identityResult)
        {
            return identityResult.Succeeded ?
                OK() :
                Error(identityResult.Errors.Select(e => e.Description));
        }
    }
}
