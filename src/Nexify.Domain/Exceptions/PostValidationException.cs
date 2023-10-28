
namespace Nexify.Domain.Exceptions
{
    public class PostValidationException : Exception
    {
        public PostValidationException(string message) : base(message) { }
    }
}
