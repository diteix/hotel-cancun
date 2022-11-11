using System.Collections.Generic;

namespace HotelCancun.Application.Dtos.Validation 
{
    public class ValidationDto<T>
    {
        public ValidationDto(params string[] validationMessage)
        {
            if (validationMessage == null || validationMessage.Length == 0)
            {
                IsValid = true;
                return;
            }

            IsValid = false;

            ValidationMessages = validationMessage;
        }

        public bool IsValid { get; private set; }

        public IEnumerable<string> ValidationMessages { get; private set; }

        public T Value { get; set; }
    }
}