using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public abstract class Validator
    {
        public abstract Task<ValidationResult> ValidateAsync(
            string fieldName,
            JToken template,
            Submission submission
        );

        /// <summary>
        /// https://stackoverflow.com/a/18870315
        /// </summary>
        /// <param name="path"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        protected object Get(string path, object instance)
        {
            var pp = path.Split('.');
            var t = instance.GetType();
            foreach (var prop in pp)
            {
                /* stop going if the parent is null */
                if (instance == null)
                    return null;

                var propInfo = t.GetProperty(prop);
                if (propInfo != null)
                {
                    instance = propInfo.GetValue(instance, null);
                    t = propInfo.PropertyType;
                }
                else throw new ArgumentException("Properties path is not correct");
            }
            return instance;
        }

        public abstract string Name { get; }
    }

    public class RequiredValidator : Validator
    {
        public override string Name => "required";

        public override Task<ValidationResult> ValidateAsync(
            string fieldName,
            JToken template,
            Submission submission)
        {
            var value = Get(fieldName, submission);

            switch (value)
            {
                case null:
                case string s when string.IsNullOrEmpty(s):
                    return Task.FromResult(new ValidationResult("Required", new[] {fieldName}));
                default:
                    return Task.FromResult(ValidationResult.Success);
            }
        }
    }
}