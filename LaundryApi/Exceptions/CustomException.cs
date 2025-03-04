

namespace LaundryBooking.Exceptions
{
    public class CustomException : Exception
    {
        public int StatusCode { get; }
        public object? ErrorDetails { get; }
        public CustomException(string message, object? errorDetails = null, int statusCode = 500) : base(message)
        {
            StatusCode = statusCode;
            ErrorDetails = errorDetails;
        }
    }
}