using System;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace AuthWithStorage.Infrastructure.Account
{
    public class PasswordGenerator
    {
        private readonly PasswordOptions _options;

        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string SpecialCharacters = "!@#$%^&*()_+-=[]{}|;:',.<>?/";

        public PasswordGenerator(IOptions<PasswordOptions> options)
        {
            _options = options.Value;
        }

        public string GeneratePassword()
        {
            var passwordLength = _options.RequiredLength;

            if (passwordLength <= 0)
                throw new InvalidConfigurationException("Password length must be greater than zero.");

            var characterPool = new StringBuilder();

            if (_options.RequireUppercase)
                characterPool.Append(Uppercase);
            if (_options.RequireLowercase)
                characterPool.Append(Lowercase);
            if (_options.RequireDigit)
                characterPool.Append(Digits);
            if (_options.RequireNonAlphanumeric)
                characterPool.Append(SpecialCharacters);

            if (characterPool.Length == 0)
                throw new InvalidOperationException("At least one character type must be included.");

            var random = new Random();
            var password = new StringBuilder();

            for (var i = 0; i < passwordLength; i++)
            {
                var randomIndex = random.Next(characterPool.Length);
                password.Append(characterPool[randomIndex]);
            }

            return password.ToString();
        }
    }
}
