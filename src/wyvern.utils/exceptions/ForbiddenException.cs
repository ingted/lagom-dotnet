namespace wyvern.api.exceptions
{
    /// <summary>
    /// Exception with 401 Forbidden
    /// </summary>
    public sealed class ForbiddenException : StatusCodeException
    {
        public ForbiddenException(string message) : base(message) { }
    }
}
