using AttechServer.Shared.Consts.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace AttechServer.Shared.ApplicationBase.Common.Validations
{
    public class CustomMaxLengthAttribute : MaxLengthAttribute
    {
        public CustomMaxLengthAttribute(int length) : base(length)
        {
        }

        public string? ErrorMessageLocalization { get; set; }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            ErrorMessage = string.Format("Độ dài trường {0} không được vượt quá {1}", validationContext.DisplayName, Length);
            return base.IsValid(value, validationContext);
        }
    }
}
