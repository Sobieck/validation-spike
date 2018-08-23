using System.Collections.Generic;
using Xunit;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp3.UnitTests
{
    public class RequiredValidatorTests
    {
        private ValidationFactory sut;
        private FormResponse template;

        public RequiredValidatorTests()
        {
            var validators = new List<Validator> { new RequiredValidator() };
            var conditions = new List<Condition> { new IsHmo() };

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
                            Name = "required",
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
                            Name = "reQuireD", 
                            Conditions = new [] { "hMo "}
                        }
                    }
                }
            };

            template = CreateFormResponse(templateConstrants);
        }

        [Fact]
        public async Task ShouldValidateWithNonEmptryString()
        {
            var submission = new Submission
            {
                FirstName = "NotNullOrEmpty"
            };

            var result = await sut.ValidateAsync(template, submission);

            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ShouldNotValidateWithNullValue()
        {
            var submission = new Submission
            {
                FirstName = null
            };

            var result = await sut.ValidateAsync(template, submission);

            Assert.NotEmpty(result.Errors);

            var error = result.Errors.First();

            Assert.Equal("FirstName", error.Field);
            Assert.Equal("Required", error.Messages.First());
        }

        [Fact]
        public async Task ShouldNotValidateWithEmptyString()
        {
            var submission = new Submission
            {
                FirstName = ""
            };

            var result = await sut.ValidateAsync(template, submission);

            Assert.NotEmpty(result.Errors);

            var error = result.Errors.First();

            Assert.Equal("FirstName", error.Field);
            Assert.Equal("Required", error.Messages.First());
        }

        [Fact]
        public async Task ShouldBeAbleToValidateNestedObjects()
        {
            var submission = new Submission
            {
                FirstName = "Roger",
                ProductType = "hMo",
                Address = new Address
                {
                    Street = ""
                }
            };

            var result = await sut.ValidateAsync(template, submission);

            Assert.NotEmpty(result.Errors);

            var error = result.Errors.First();

            Assert.Equal("Address.Street", error.Field);
            Assert.Equal("Required", error.Messages.First());
        }

        [Fact]
        public async Task ShouldNotLeaveAnyMessageOnValidObjects()
        {
            var submission = new Submission
            {
                FirstName = "Roger",
                ProductType = "hMo",
                Address = new Address
                {
                    Street = "Fake Way"
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