using AuthWithStorage.Domain;

namespace AuthWithStorage.Application.DTOs
{
    /// <summary>
    /// Represents a user data transfer object.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; }
    }

    /// <summary>
    /// Represents a detailed response for a user, including additional metadata.
    /// </summary>
    public class UserResponse : UserDto
    {
        /// <summary>
        /// Gets or sets the date and time when the user was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user was last updated, if applicable.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the role assigned to the user.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the permissions assigned to the user.
        /// </summary>
        public PermissionType[] Permissions { get; set; }
    }
}
