using System.Text;
using System.Security.Cryptography;

namespace AuthWithStorage.Infrastructure.Account
{
    /// <summary>
    /// Provides methods for hashing and verifying passwords.
    /// </summary>
    public sealed class PasswordHasher
    {
        /// <summary>
        /// Hashes a password using HMACSHA256 and returns the salt and hash as a single string.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>A string containing the salt and hash, separated by a colon.</returns>
        public string HashPassword(string password)
        {
            using var hmac = new HMACSHA256();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verifies a password against a hashed password.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="hashedPassword">The hashed password to compare against.</param>
        /// <returns>True if the password matches the hash; otherwise, false.</returns>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            var parts = hashedPassword.Split(':');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = Convert.FromBase64String(parts[1]);

            using var hmac = new HMACSHA256(salt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
}
