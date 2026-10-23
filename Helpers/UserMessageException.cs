namespace Suvarna.DGMS.Api.Helpers
{
    /// <summary>
    /// NEW: a business-rule message meant for the user (for example "Please select a bed").
    /// GlobalExceptionMiddleware shows its text as it is, also in production.
    /// Any other exception still shows only "Unexpected error. Reference Id: ...".
    /// </summary>
    public class UserMessageException : Exception
    {
        public UserMessageException(string message) : base(message) { }
    }
}
