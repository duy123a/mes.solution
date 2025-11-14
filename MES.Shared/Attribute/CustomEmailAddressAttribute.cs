using EmailValidation;
using System.ComponentModel.DataAnnotations;

namespace MES.Shared.Attribute
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class CustomEmailAddressAttribute : DataTypeAttribute
    {
        public CustomEmailAddressAttribute() : base(DataType.EmailAddress) { }

        public override bool IsValid(object? value)
        {
            if (value is not string valueAsString || valueAsString.Length > 320)
                return false;

            return EmailValidator.Validate(valueAsString);
        }
    }
}
