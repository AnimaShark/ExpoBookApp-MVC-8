using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ExpoBookApp.Services
{
    public class ComplexPasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var password = value as string;

            if (string.IsNullOrWhiteSpace(password))
                return false;

            // At least 8 characters, one uppercase, one lowercase, one number, and one special character
            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";

            return Regex.IsMatch(password, pattern);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be at least 8 characters and include an uppercase letter, a lowercase letter, a number, and a special character.";
        }
    }
}
