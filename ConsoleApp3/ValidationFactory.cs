using Newtonsoft.Json.Linq;
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
            var templates = template
                .Sections
                .SelectMany(x => x.Groups)
                .SelectMany(x => x.Fields)
                .ToList();

            var fields = new List<ValidationFields>();

            foreach (var templateField in templates)
            {
                var validatorAndParams = new List<ValidatorAndParameters>();

                foreach (var validatorDefinition in templateField.Validators)
                {
                    var validator = validators.First(x => x.Name.Equals(validatorDefinition.Name, StringComparison.OrdinalIgnoreCase));

                    var shouldBeFilteredByConditions = validatorDefinition
                        .Conditions
                        .All(x =>
                            conditions.Where(y =>
                                y.Name.Equals(x.Trim(), StringComparison.OrdinalIgnoreCase))
                            .Any(condition => condition.Evaluate(submission)));

                    if (!shouldBeFilteredByConditions)
                    {
                        continue;
                    }

                    validatorAndParams.Add(new ValidatorAndParameters
                    {
                        Parameters = validatorDefinition.Parameters,
                        Validator = validators.First(x => x.Name.Equals(validatorDefinition.Name, StringComparison.OrdinalIgnoreCase))
                    });
                }
                
                fields.Add(new ValidationFields
                {
                    Name = templateField.Name,
                    ValidatorAndParameters = validatorAndParams
                });
            }


            var validationMessages = new ValidationMessages();

            foreach (var field in fields)
            {
                var messages = new List<string>();
                foreach (var validatorAndParameters in field.ValidatorAndParameters)
                {
                    var result = await validatorAndParameters.Validator.ValidateAsync(field.Name, validatorAndParameters.Parameters, submission);
                    if (result == null) continue;

                    messages.Add(result.ErrorMessage);
                }

                if (messages.Count > 0)
                {
                    validationMessages.Errors.Add(new ValidationMessage
                    {
                        Field = field.Name,
                        Messages = messages
                    });
                }
            }

            return validationMessages;
        }

        private class ValidationFields
        {
            public string Name { get; set; }
            public ICollection<ValidatorAndParameters> ValidatorAndParameters { get; set; }
        }

        private class ValidatorAndParameters
        {
            public JToken Parameters { get; set; }
            public Validator Validator { get; set; }
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