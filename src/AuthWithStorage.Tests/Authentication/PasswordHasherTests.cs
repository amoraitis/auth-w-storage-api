using AuthWithStorage.Infrastructure.Account;

namespace AuthWithStorage.Tests.Authentication
{
    public class PasswordHasherTests
    {
        [Fact]
        public void HashPassword_ShouldReturnNonEmptyString()
        {
            // Arrange
            var passwordHasher = new PasswordHasher();
            var password = "TestPassword123";

            // Act
            var hashedPassword = passwordHasher.HashPassword(password);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(hashedPassword));
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrueForValidPassword()
        {
            // Arrange
            var passwordHasher = new PasswordHasher();
            var password = "emkuQ3>!oVNq";
            var hashedPassword = passwordHasher.HashPassword(password);

            // Act
            var result = passwordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalseForInvalidPassword()
        {
            // Arrange
            var passwordHasher = new PasswordHasher();
            var password = "TestPassword123";
            var hashedPassword = passwordHasher.HashPassword(password);
            var invalidPassword = "WrongPassword";

            // Act
            var result = passwordHasher.VerifyPassword(invalidPassword, hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalseForMalformedHashedPassword()
        {
            // Arrange
            var passwordHasher = new PasswordHasher();
            var password = "TestPassword123";
            var malformedHashedPassword = "InvalidFormat";

            // Act
            var result = passwordHasher.VerifyPassword(password, malformedHashedPassword);

            // Assert
            Assert.False(result);
        }
    }
}
