using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Efmap.Validation
{
    public static class ValidationHelper
    {
        public static Tuple<bool, List<ValidationResult>> IsValid<T>(T source)
        {
            var context = new ValidationContext(source, serviceProvider: null, items: null);
            var errors = new List<ValidationResult>();
            var validity = Validator.TryValidateObject(source, context, errors);
            return new Tuple<bool, List<ValidationResult>>(validity, errors);
        }
    }
}
