using Microsoft.AspNetCore.Identity;
namespace API.Controllers.User
{
    public class CustomUserValidator<TUser> : IUserValidator<TUser> where TUser : class
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            var errors = new List<IdentityError>();
            var userName = await manager.GetUserNameAsync(user);

            if (string.IsNullOrWhiteSpace(userName))
            {
                errors.Add(new IdentityError
                {
                    Code = "UserNameIsEmpty",
                    Description = "User name cannot be empty."
                });
            }
            else
            {
                if (userName.Length < 6)
                {
                    errors.Add(new IdentityError
                    {
                        Code = "UserNameTooShort",
                        Description = "User name must be at least 6 characters long."
                    });
                }

                if (!userName.Any(char.IsUpper))
                {
                    errors.Add(new IdentityError
                    {
                        Code = "UserNameNoUpperCase",
                        Description = "User name must contain at least one uppercase letter."
                    });
                }

                var spaceCount = userName.Count(c => c == ' ');
                if (spaceCount != 1)
                {
                    errors.Add(new IdentityError
                    {
                        Code = "UserNameInvalidSpaces",
                        Description = "User name must contain exactly one whitespace."
                    });
                }
            }

            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }

}