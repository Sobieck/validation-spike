using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class ValidationFactory
    {
        private readonly IEnumerable<Validator> validators;
        private readonly IEnumerable<Condition> conditions;

        public ValidationFactory(
            IEnumerable<Validator> validators,
            IEnumerable<Condition> conditions
        )
        {
            this.validators = validators;
            this.conditions = conditions;
        }

        public async Task<ValidationMessages> ValidateAsync(
            FormResponse template,
            Submission submission
        )
        {
            var fields = template
                .Sections
                .SelectMany(x => x.Groups)
                .SelectMany(x => x.Fields)
                .Select(field => new
                {
                    name = field.Name,
                    validators = field.Validators
                        .SelectMany
                        (
                            definition => validators
                                .Where(y => y.Name == definition.Name && 
                                            definition.Conditions.All(c => conditions.Where(co => co.Name == c).Any(p => p.Evaluate(submission))))
                                .ToList()
                        )
                })
                .ToList();

            var validationMessages = new ValidationMessages();

            foreach (var field in fields)
            {
                var messages = new List<string>();
                foreach (var validator in field.validators)
                {
                    var result = await validator.ValidateAsync(field.name, template, submission);
                    if (result == null) continue;

                    messages.Add(result.ErrorMessage);
                }

                if (messages.Count > 0)
                {
                    validationMessages.Errors.Add(new ValidationMessage
                    {
                        Field = field.name,
                        Messages = messages
                    });
                }
            }

            return validationMessages;
        }
    }

    public abstract class Condition
    {
        public abstract string Name {get;}
        public abstract bool Evaluate(Submission submission);
    }

    public class IsHmo : Condition
    {
        public override string Name  => "hmo";

        public override bool Evaluate(Submission submission)
        {
            return submission?.ProductType?.Equals("HMO", StringComparison.OrdinalIgnoreCase) == true;
        }
    }


}