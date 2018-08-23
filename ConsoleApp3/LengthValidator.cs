using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class LengthValidator : Validator
    {
        public override string Name => "length";

        public override Task<ValidationResult> ValidateAsync(string fieldName, FormResponse template, Submission submission)
        {
            var value = Get(fieldName, submission);
            var min = GetLengthContraint(fieldName, template, "min");
            var max = GetLengthContraint(fieldName, template, "max");
            
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

        private int? GetLengthContraint(string fieldName, FormResponse template, string paramName)
        {
            var lengthParameters = template
                            .Sections
                            .SelectMany(x => x.Groups)
                            .SelectMany(x => x.Fields)
                            .First(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                            .Validators
                            .First(x => x.Name.Equals(Name, StringComparison.OrdinalIgnoreCase))
                            .Parameters;

            var minToken = lengthParameters[paramName]?.ToString();

            if (int.TryParse(minToken, out int result))
            {
                return result;
            }

            return null;
        }
    }
}