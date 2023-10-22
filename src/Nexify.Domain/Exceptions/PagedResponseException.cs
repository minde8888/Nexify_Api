
namespace Nexify.Domain.Exceptions
{
    public class PagedResponseException : Exception
    {
        public PagedResponseException(string message) : base(message)
        { }
    }
}
