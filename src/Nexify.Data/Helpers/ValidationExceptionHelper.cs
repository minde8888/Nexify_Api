using FluentValidationValidationResult = FluentValidation.Results.ValidationResult;

namespace Nexify.Data.Helpers
{
    public static class ValidationExceptionHelper
    {
        public static void ThrowIfInvalid<T>(FluentValidationValidationResult validationResult) where T : Exception
        {
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors
                    .Select(error => $"{error.PropertyName}: {error.ErrorMessage}")
                    .ToList();

                var exceptionMessage = string.Join(Environment.NewLine, errorMessages);

                throw (T)Activator.CreateInstance(typeof(T), exceptionMessage);
            }
        }
    }
}

