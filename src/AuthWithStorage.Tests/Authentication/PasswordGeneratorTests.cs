using AuthWithStorage.Infrastructure.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AuthWithStorage.Tests.Authentication
{
    public class PasswordGeneratorTests
    {
        private PasswordGenerator CreatePasswordGenerator(PasswordOptions options)
        {
            var optionsWrapper = Options.Create(options);
            return new PasswordGenerator(optionsWrapper);
        }

        [Fact]
        public void GeneratePassword_ShouldGeneratePasswordWithRequiredLength()
        {
            // Arrange
            var options = new PasswordOptions { RequiredLength = 12 };
            var generator = CreatePasswordGenerator(options);

            // Act
            var password = generator.GeneratePassword();

            // Assert
            Assert.NotNull(password);
            Assert.Equal(12, password.Length);
        }

        [Fact]
        public void GeneratePassword_ShouldThrowException_WhenNoCharacterTypesAreIncluded()
        {
            // Arrange
            var options = new PasswordOptions
            {
                RequireUppercase = false,
                RequireLowercase = false,
                RequireDigit = false,
                RequireNonAlphanumeric = false
            };
            var generator = CreatePasswordGenerator(options);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => generator.GeneratePassword());
        }

        [Fact]
        public void GeneratePassword_ShouldIncludeUppercaseCharacters_WhenOptionIsEnabled()
        {
            // Arrange
            var options = new PasswordOptions
            {
                RequireUppercase = true,
                RequireLowercase = false,
                RequireDigit = false,
                RequireNonAlphanumeric = false
            };
            var generator = CreatePasswordGenerator(options);

            // Act
            var password = generator.GeneratePassword();

            // Assert
            Assert.NotNull(password);
            Assert.Contains(password, c => "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(c));
        }

        [Fact]
        public void GeneratePassword_ShouldIncludeLowercaseCharacters_WhenOptionIsEnabled()
        {
            // Arrange
            var options = new PasswordOptions
            {
                RequireUppercase = false,
                RequireLowercase = true,
                RequireDigit = false,
                RequireNonAlphanumeric = false
            };
            var generator = CreatePasswordGenerator(options);

            // Act
            var password = generator.GeneratePassword();

            // Assert
            Assert.NotNull(password);
            Assert.Contains(password, c => "abcdefghijklmnopqrstuvwxyz".Contains(c));
        }

        [Fact]
        public void GeneratePassword_ShouldIncludeDigits_WhenOptionIsEnabled()
        {
            // Arrange
            var options = new PasswordOptions
            {
                RequireUppercase = false,
                RequireLowercase = false,
                RequireDigit = true,
                RequireNonAlphanumeric = false
            };
            var generator = CreatePasswordGenerator(options);

            // Act
            var password = generator.GeneratePassword();

            // Assert
            Assert.NotNull(password);
            Assert.Contains(password, c => "0123456789".Contains(c));
        }

        [Fact]
        public void GeneratePassword_ShouldIncludeSpecialCharacters_WhenOptionIsEnabled()
        {
            // Arrange
            var options = new PasswordOptions
            {
                RequireUppercase = false,
                RequireLowercase = false,
                RequireDigit = false,
                RequireNonAlphanumeric = true
            };
            var generator = CreatePasswordGenerator(options);

            // Act
            var password = generator.GeneratePassword();

            // Assert
            Assert.NotNull(password);
            Assert.Contains(password, c => "!@#$%^&*()_+-=[]{}|;:',.<>?/".Contains(c));
        }

        [Fact]
        public void GeneratePassword_ShouldGeneratePasswordWithAllCharacterTypes()
        {
            // Arrange
            var options = new PasswordOptions
            {
                RequiredLength = 12,
                RequireUppercase = true,
                RequireLowercase = true,
                RequireDigit = true,
                RequireNonAlphanumeric = true
            };
            var generator = CreatePasswordGenerator(options);

            // Act
            var password = generator.GeneratePassword();

            // Assert
            Assert.NotNull(password);
            Assert.Equal(12, password.Length);
            Assert.Contains(password, c => "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(c));
            Assert.Contains(password, c => "abcdefghijklmnopqrstuvwxyz".Contains(c));
            Assert.Contains(password, c => "0123456789".Contains(c));
            Assert.Contains(password, c => "!@#$%^&*()_+-=[]{}|;:',.<>?/".Contains(c));
        }
    }
}
