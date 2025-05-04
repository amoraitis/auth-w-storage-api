namespace AuthWithStorage.Application.DTOs
{
    /// <summary>
    /// Represents a request for user login containing the necessary credentials.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the username of the user attempting to log in.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password of the user attempting to log in.
        /// </summary>
        public string Password { get; set; }
    }
}
