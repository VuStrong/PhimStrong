using Microsoft.AspNetCore.Identity;
using PhimStrong.Domain.Models;

namespace PhimStrong.Application.Models
{
	public class RegisterResult : Result
	{
		public User? User { get; set; }

		public new static RegisterResult OK() => new RegisterResult { Success = true };

		public new static RegisterResult Error(params string[] errors)
		{
			return new RegisterResult
			{
				Success = false,
				Errors = errors.ToList()
			};
		}

		public new static RegisterResult ToAppResult(IdentityResult identityResult)
		{
			return identityResult.Succeeded ?
				OK() :
				Error(identityResult.Errors.Select(e => e.Description).ToArray());
		}
	}
}
