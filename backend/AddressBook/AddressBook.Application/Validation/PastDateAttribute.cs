using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AddressBook.Application.Validation
{
    /// <summary>
    /// Requires a date strictly earlier than today. DataAnnotations has no built-in
    /// equivalent, and <see cref="RangeAttribute"/> cannot express a moving upper bound.
    /// Null passes so that <see cref="RequiredAttribute"/> owns the "missing" case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is not DateTime date)
            {
                return new ValidationResult($"{validationContext.DisplayName} must be a date.");
            }

            if (date.Date >= DateTime.Today)
            {
                return new ValidationResult(
                    ErrorMessage ?? $"{validationContext.DisplayName} must be a date in the past.",
                    validationContext.MemberName is null ? null : new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
