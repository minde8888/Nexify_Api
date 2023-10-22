
namespace Nexify.Domain.Exceptions
{
    public class PaginationValidationException : Exception
    {
        public PaginationValidationException(string message) : base(message)
        { }
    }
}
