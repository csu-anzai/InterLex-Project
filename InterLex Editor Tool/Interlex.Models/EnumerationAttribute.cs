using System;
using System.Collections.Generic;
using System.Text;

namespace Interlex.Models
{
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property)]
    internal class EnumerationAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyType = validationContext.ObjectType.GetProperty(validationContext.MemberName).PropertyType;
            var values = Enum.GetValues(propertyType);
            var unboxed = (int)value;
            var valid = false;
            foreach (var option in values)
            {
                if ((int)option == unboxed)
                {
                    valid = true;
                    break;
                }
            }

            if (!valid)
            {
                return new ValidationResult($"Property {validationContext.DisplayName} has an invalid value.");
            }

            return ValidationResult.Success;
        }
    }
}
