using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var template = CreateFormResponse();
            var submission = new Submission
            {
                ProductType = "hmo",
                FirstName = "",
                Address =
                {
                    Street = "1 Fantasy St"
                }
            };

            var factory = new ValidationFactory(
                validators: new[] {
                    new RequiredValidator()
                },
                conditions: new[] {
                    new IsHmo()
                });

            var messages = await factory.ValidateAsync(template, submission);

        }

        private static FormResponse CreateFormResponse()
        {
            return new FormResponse
            {
                Sections = new[]
                {
                    new SectionResponse
                    {
                        Groups = new[]
                        {
                            new FieldGroupResponse
                            {
                                Fields = new[]
                                {
                                    new FieldResponse
                                    {
                                        Name = "FirstName",
                                        Validators = new[]
                                        {
                                            new ValidatorDefinition
                                            {
                                                Name = "required",
                                                Conditions = new [] { "hmo" }
                                            }
                                        },
                                    },
                                    new FieldResponse
                                    {
                                        Name = "Address.Street",
                                        Validators = new[]
                                        {
                                            new ValidatorDefinition
                                            {
                                                Name = "required"
                                            }
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

    }
}
