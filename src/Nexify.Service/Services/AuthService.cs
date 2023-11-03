using FluentValidation;
using Nexify.Domain.Entities.Auth;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Interfaces;
using System.Web;

namespace Nexify.Service.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IAuthServiceWrap _authServiceWrap;
        private readonly IEmailService _emailService;

        private readonly IValidator<Signup> _signupValidator;
        private readonly IValidator<Login> _loginValidator;
        private readonly IValidator<ForgotPassword> _forgotPasswordValidator;
        private readonly IValidator<ResetPassword> _resetPasswordValidator;

        public AuthService(
                       IAuthRepository authRepository,
                                  ITokenService tokenService,
                                             IAuthServiceWrap authServiceWrap,
                                                        IEmailService emailService,
                                                                   IValidator<Signup> signupValidator,
                                                                              IValidator<Login> loginValidator,
                                                                                         IValidator<ForgotPassword> forgotPasswordValidator,
                                                                                                    IValidator<ResetPassword> resetPasswordValidator)
        {
            _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _authServiceWrap = authServiceWrap ?? throw new ArgumentNullException(nameof(authServiceWrap));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _signupValidator = signupValidator ?? throw new ArgumentNullException(nameof(signupValidator));
            _loginValidator = loginValidator ?? throw new ArgumentNullException(nameof(loginValidator));
            _forgotPasswordValidator = forgotPasswordValidator ?? throw new ArgumentNullException(nameof(forgotPasswordValidator));
            _resetPasswordValidator = resetPasswordValidator ?? throw new ArgumentNullException(nameof(resetPasswordValidator));
        }

        public async Task<SignupResponse> CreateUserAsync(Signup user)
        {
            var validationResult = await _signupValidator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                return new SignupResponse()
                {
                    Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList(),
                    Success = false
                };
            }

            if (_authRepository.UserExistAsync(user.PhoneNumber, user.Email))
                throw new UserException("Email or phone number is already exist");

            var newUser = new ApplicationUser()
            {
                Roles = user.Roles,
                Email = user.Email,
                UserName = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            var result = await _authRepository.CreateUserAsync(newUser, user.Password);

            if (!result.Succeeded)
            {
                return new SignupResponse()
                {
                    Errors = result.Errors.Select(x => x.Description).ToList(),
                    Success = false
                };
            }

            await _authRepository.AddRoleAsync(newUser, user.Roles);

            return new SignupResponse()
            {
                Success = result.Succeeded
            };
        }

        public async Task<AuthResults> AuthAsync(Login login)
        {
            var validationResult = await _loginValidator.ValidateAsync(login);

            if (!validationResult.IsValid)
            {
                return new AuthResults
                {
                    Errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList(),
                    Success = false
                };
            }

            var user = await _authRepository.AuthUserAsync(login.Email) ?? throw new UserException("User not found please try again");

            if (user.IsDeleted)
                throw new UserException("User not found please try again");

            var isPasswordValid = await _authRepository.PasswordValidatorAsync(user, login.Password);

            if (!isPasswordValid)
                throw new UserException("User not found please try again");

            return await _tokenService.GenerateJwtTokenAsync(user);
        }

        public async Task<AuthResults> SendResetPassword(ForgotPassword forgotPassword, string url)
        {
            var validationResult = await _forgotPasswordValidator.ValidateAsync(forgotPassword);
            if (!validationResult.IsValid)
            {
                return new AuthResults()
                {
                    Errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList(),
                    Success = false
                };
            }
            var user = await _authServiceWrap.FindUserAsync(forgotPassword.Email)
                ?? throw new UserException("User not found please try again");

            var token = await _authServiceWrap.TokenAsync(user);
            if (string.IsNullOrEmpty(token))
                throw new TokenException("Token is null");

            user.ResetToken = token;
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);
            await _authServiceWrap.UpdateUserAsync(user);

            var link = $"{url}/api/Auth/NewPassword?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(user.Email)}";
            var subject = "Web Shop";
            var htmlContent = string.Empty;
            var plainTextContent = $"Click the link below to reset your password:<br/><a href={link}><strong>click here</strong></a>";

            var sendEmailTask = await _emailService.SendEmailAsync(forgotPassword.Email, subject, plainTextContent, htmlContent);
            return new AuthResults()
            {
                Success = sendEmailTask
            };
        }

        public async Task<AuthResults> GetPasswordAsync(ResetPassword resetPassword)
        {
            var validationResult = await _resetPasswordValidator.ValidateAsync(resetPassword);
            if (!validationResult.IsValid)
            {
                return new AuthResults()
                {
                    Errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList(),
                    Success = false
                };
            }

            var user = await _authServiceWrap.FindUserAsync(resetPassword.Email);
            if (user == null || (user.ResetToken != resetPassword.Token && user.ResetTokenExpires < DateTime.UtcNow))
            {
                return new AuthResults()
                {
                    Success = false
                };
            }

            var resetPassResult = await _authServiceWrap.NewPasswordAsync(user, resetPassword.Token, resetPassword.Password);
            if (!resetPassResult.Succeeded)
            {
                throw new PasswordException("Failed to reset password");
            }

            user.PasswordReset = DateTime.UtcNow;
            user.ResetToken = null;
            user.ResetTokenExpires = null;
            await _authServiceWrap.UpdateUserAsync(user);

            return new AuthResults()
            {
                Success = true
            };
        }
    }
}
