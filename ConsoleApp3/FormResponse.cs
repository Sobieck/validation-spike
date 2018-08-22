using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp3
{
    public class FormResponse
    {
        /// <summary>
        /// Each indendent section on a form
        /// </summary>
        public IEnumerable<SectionResponse> Sections { get; set; }
            = Array.Empty<SectionResponse>();
    }

    public class SectionResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Grouping of fields (something like rows, but not really)
        /// </summary>
        public IEnumerable<FieldGroupResponse> Groups { get; set; }
            = Array.Empty<FieldGroupResponse>();
    }

    public class FieldGroupResponse
    {
        public IEnumerable<FieldResponse> Fields { get; set; } 
            = Array.Empty<FieldResponse>();
    }

    public class FieldResponse
    {
        /// <summary>
        /// The programmatic name of the field
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// What a user will see
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// The type of the field i.e. text, radio, checkbox
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Triggers work by specifying the name of a Section or
        /// the name of a field. If the value of this field is
        /// truthy then the trigger should show the target
        /// </summary>        
        public string[] TriggerTargets { get; set; }
        /// <summary>
        /// Listeners work by specifying the name of an event
        /// which the ui can listen for to handle. The event and handling
        /// of the event is dependent on the UI implementation
        /// </summary>        
        public string[] Listeners { get; set; }
        /// <summary>
        /// Set the default value of a field
        /// </summary>
        public string Default { get; set; }
        /// <summary>
        /// Tier of field: Values between 1 (severe), 2 (important), 3 (nice to have)
        /// Defaults to 1 (Severe)
        /// </summary>
        public int Tier { get; set; } = 1;
        /// <summary>
        /// In the chance that the field is a select or multi-select
        /// you can use this additional set of options to populate
        /// the field
        /// </summary>
        public IList<FieldSelectOptionResponse> Options { get; set; }
            = Array.Empty<FieldSelectOptionResponse>();

        public IList<ValidatorDefinition> Validators { get; set; }
            = new List<ValidatorDefinition>();
    }

    public class FieldSelectOptionResponse
    {
        public string Display { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string[] TriggerTargets { get; set; }
    }
}
