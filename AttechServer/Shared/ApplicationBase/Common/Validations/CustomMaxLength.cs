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
            ErrorMessage = string.Format("�? d�i tru?ng {0} kh�ng du?c vu?t qu� {1}", validationContext.DisplayName, Length);
            return base.IsValid(value, validationContext);
        }
    }
}
