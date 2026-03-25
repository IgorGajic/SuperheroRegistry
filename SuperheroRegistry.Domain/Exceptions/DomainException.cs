namespace SuperheroRegistry.Domain.Exceptions
{
    /// <summary>
    /// Base exception for domain-related errors in the superhero registry system.
    /// Raised when domain rules or business logic constraints are violated.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Gets the error code identifying the type of domain violation.
        /// </summary>
        public string? ErrorCode { get; }

        public DomainException()
        {
        }

        public DomainException(string? message) : base(message)
        {
        }

        public DomainException(string? message, string? errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public DomainException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public DomainException(string? message, string? errorCode, Exception? innerException) 
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
