namespace DustInTheWind.RequestR;

/// <summary>
/// Exception thrown when the requested type is different from the actual type of the response
/// and a cast cannot be performed.
/// </summary>
public class ResponseCastException : RequestRException
{
    private const string DefaultMessageTemplate = "Use case returned {0} that cannot be converted to {1}";

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseCastException"/> class.
    /// </summary>
    /// <param name="actualResponseType">The type of the actual response.</param>
    /// <param name="requestedResponseType">The type requested to be returned.</param>
    /// <param name="inner">The exception that generated the current one.</param>
    public ResponseCastException(Type actualResponseType, Type requestedResponseType, Exception inner)
        : base(string.Format(DefaultMessageTemplate, actualResponseType.FullName, requestedResponseType.FullName), inner)
    {
    }
}