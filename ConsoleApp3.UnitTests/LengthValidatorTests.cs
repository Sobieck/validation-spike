using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleApp3.UnitTests
{
    public class LengthValidatorTests
    {
        private ValidationFactory sut;
        private FormResponse template;

        public LengthValidatorTests()
        {
            var validators = new List<Validator> { new LengthValidator(), new RequiredValidator() };
            var conditions = new List<Condition> { };

            sut = new ValidationFactory(validators, conditions);

            var templateConstrants = new[]
            {
                new FieldResponse
                {
                    Name = "FirstName",
                    Validators = new[]
                    {
                        new ValidatorDefinition
                        {
                            Name = "length",
                            Parameters =  JToken.Parse("{ min : 10, max: 20 }")
                        },
                        new ValidatorDefinition
                        {
                            Name = "required"
                        }
                    },
                },
                new FieldResponse
                {
                    Name = "LastName",
                    Validators = new[]
                    {
                        new ValidatorDefinition
                        {
                            Name = "length",
                            Parameters = JToken.Parse("{ min: 2 }")
                        }
                    }
                },
                new FieldResponse
                {
                    Name = "Address.Street",
                    Validators = new[]
                    {
                        new ValidatorDefinition
                        {
                            Name = "length",
                            Parameters = JToken.Parse("{ max: 5}")
                        }
                    }
                },
                new FieldResponse
                {
                    Name = "Address.ZipCode"
                }

            };

            template = CreateFormResponse(templateConstrants);
        }

        [Fact]
        public async Task NullValuesShouldBreakIfTheMinimumCharacterCountIsSet()
        {
            var submission = new Submission
            {
                LastName = null
            };

            var result = await sut.ValidateAsync(template, submission);

            Assert.NotEmpty(result.Errors);

            var error = result
                .Errors
                .FirstOrDefault(x => 
                    x.Field.Equals("LastName", StringComparison.OrdinalIgnoreCase));

            Assert.Equal("Length: Min 2", error.Messages.First());
        }

        [Fact]
        public async Task MultipleValidorsShouldBeAbleToLeaveMessages()
        {
            var submission = new Submission
            {
            };

            var result = await sut.ValidateAsync(template, submission);

            Assert.NotEmpty(result.Errors);

            var error = result
                .Errors
                .FirstOrDefault(x =>
                    x.Field.Equals("FirstName", StringComparison.OrdinalIgnoreCase));

            Assert.Equal(2, error.Messages.Count);
            Assert.True(error.Messages.Contains("Required"));
            Assert.True(error.Messages.Contains("Length: Min 10"));
        }

        [Fact]
        public async Task LengthValidatorShouldNotBeTriggeredIfValidatorOnlyCaresAboutMax()
        {
            var submission = new Submission
            {
            };

            var result = await sut.ValidateAsync(template, submission);

            var error = result
                .Errors
                .FirstOrDefault(x =>
                    x.Field.Equals("Address.Street", StringComparison.OrdinalIgnoreCase));

            Assert.Null(error);
        }

        [Fact]
        public async Task TheMinimumValidatorShouldWork()
        {
            var submission = new Submission
            {
                LastName = "1"
            };

            var result = await sut.ValidateAsync(template, submission);

            var error = result
                .Errors
                .FirstOrDefault(x =>
                    x.Field.Equals("LastName", StringComparison.OrdinalIgnoreCase));

            Assert.Equal(1, error.Messages.Count);
            Assert.True(error.Messages.Contains("Length: Min 2"));
        }

        [Fact]
        public async Task TheMaximumShouldBeRespected()
        {
            var submission = new Submission
            {
                FirstName = "123456789012345678901234567890"
            };

            var result = await sut.ValidateAsync(template, submission);

            var error = result
                .Errors
                .FirstOrDefault(x =>
                    x.Field.Equals("FirstName", StringComparison.OrdinalIgnoreCase));

            Assert.Equal(1, error.Messages.Count);
            Assert.True(error.Messages.Contains("Length: Max 20"));
        }

        [Fact]
        public async Task EveryLengthFieldShouldBeValid()
        {
            var submission = new Submission
            {
                FirstName = "1234567890",
                LastName = "Ab",
                Address = new Address
                {
                    Street = "Old R"
                }
            };

            var result = await sut.ValidateAsync(template, submission);

            Assert.Empty(result.Errors);
        }

        private FormResponse CreateFormResponse(IEnumerable<FieldResponse> fieldResponses)
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
                                Fields = fieldResponses
                            }
                        }
                    }
                }
            };
        }
    }
}
