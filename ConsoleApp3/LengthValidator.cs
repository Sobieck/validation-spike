using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class LengthValidator : Validator
    {
        public override string Name => "length";

        public override Task<ValidationResult> ValidateAsync(string fieldName, JToken parameters, Submission submission)
        {
            var value = Get(fieldName, submission);
            var min = GetLengthContraint(fieldName, parameters, "min");
            var max = GetLengthContraint(fieldName, parameters, "max");
            
            if (value == null && min != null)
            {
                return Task.FromResult(new ValidationResult($"Length: Min {min}"));
            }

            if (value == null && min == null)
            {
                return Task.FromResult(ValidationResult.Success);
            }

            var valueLength = (value as string).Length;

            if (min != null && valueLength < min)
            {
                return Task.FromResult(new ValidationResult($"Length: Min {min}"));
            }

            if(max != null && max < valueLength)
            {
                return Task.FromResult(new ValidationResult($"Length: Max {max}"));
            }

            return Task.FromResult(ValidationResult.Success);
        }

        private int? GetLengthContraint(string fieldName, JToken parameters, string paramName)
        {
  
            var minToken = parameters[paramName]?.ToString();

            if (int.TryParse(minToken, out int result))
            {
                return result;
            }

            return null;
        }
    }
}