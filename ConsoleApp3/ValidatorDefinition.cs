using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ConsoleApp3
{
    /// <summary>
    /// { "name" : "", "parameters" : {}, "conditions" : [ ] }
    /// </summary>
    public class ValidatorDefinition
    {
        public string Name { get; set; }
        public JToken Parameters { get; set; }
        public ICollection<string> Conditions { get; set; } = new List<string>();
    }

    public class ValidationMessages
    {
        public string Message { get; set; }
        public ICollection<ValidationMessage> Errors { get; set; } = new List<ValidationMessage>();
        public ICollection<ValidationMessage> Warnings { get; set; } = new List<ValidationMessage>();
        public ICollection<ValidationMessage> Informationals { get; set; } = new List<ValidationMessage>();
    }

    public class ValidationMessage
    {
        public string Field { get; set; }
        public ICollection<string> Messages { get; set; }
    }
}