using AttechServer.Shared.Consts.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace AttechServer.Shared.ApplicationBase.Common.Validations
{
    /// <summary>
    /// Cho phép m?t trong các giá tr?, n?u là null thì b? qua
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
            var msg = $"Vui lòng ch?n 1 trong các giá tr? sau: {string.Join(", ", AllowableValues?.Select(i => i.ToString()).ToArray() ?? new string[] { "Không có giá tr? nào du?c phép" })}.";
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                msg = ErrorMessage;
            }
            return new ValidationResult(msg);
        }
    }
}
