using AttechServer.Shared.Consts.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace AttechServer.Shared.ApplicationBase.Common.Validations
{
    /// <summary>
    /// Cho ph�p m?t trong c�c gi� tr?, n?u l� null th� b? qua
    /// </summary>
    public class IntegerRangeAttribute : ValidationAttribute
    {
        public int[] AllowableValues { get; set; } = null!;

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || AllowableValues?.Contains((int)value) == true)
            {
                return ValidationResult.Success;
            }
            var msg = $"Vui l�ng ch?n 1 trong c�c gi� tr? sau: {string.Join(", ", AllowableValues?.Select(i => i.ToString()).ToArray() ?? new string[] { "Kh�ng c� gi� tr? n�o du?c ph�p" })}.";
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                msg = ErrorMessage;
            }
            return new ValidationResult(msg);
        }
    }
}
