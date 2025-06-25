using ExpoBookApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ExpoBookApp.Services
{
    public static class EventValidator
    {
        public static ValidationResult ValidateEndDate(DateTime endDate, ValidationContext context)
        {
            var instance = context.ObjectInstance as Event;
            if (instance != null && endDate < instance.StartDate)
            {
                return new ValidationResult("End date must be after start date");
            }
            return ValidationResult.Success;
        }
    }
}
