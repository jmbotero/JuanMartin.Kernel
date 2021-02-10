using JuanMartin.Kernel.Attributes;
using JuanMartin.Kernel.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace JuanMartin.Kernel.Processors
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CommandLineOption
    {
        public enum OptionStatus
        {
            [StringValue("defined")]
            defined = 1,
            [StringValue("initialized")]
            initialized = 2,
            [StringValue("assigned")]
            assigned = 3
        };

        [JsonProperty]
        public string Name { get; private set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public OptionStatus Status { get; set; }
        [JsonProperty]
        public object Value { get; set; }
        [JsonProperty]
        public string ValueType { get; set; }
        [JsonProperty]
        public string Abbreviation { get; private set; }
        [JsonProperty]
        public List<string> Dependencies { get; set; }
        [JsonProperty]
        public bool IsSingle { get; private set; }
        [JsonProperty]
        public bool IsRequired { get; set; }
        public bool IsStandAlone { get; private set; }


        public CommandLineOption(string name, string value, string valuetype, bool? isrequired, List<string> dependencies, string abbreviation = "")
        {
            Name = name;
     
            if (value != string.Empty)
            {
                IsSingle = false;
                Value = value;
            }
            else
            {
                IsSingle = true;
                Value = null;
            }
            if (abbreviation == string.Empty)
            {
                abbreviation = Name.ToLower().Substring(0, 1);
            }
            Abbreviation = abbreviation;
            
            if (dependencies == null)
                Dependencies = new List<string>();
            else
                Dependencies = dependencies;

            IsRequired = false;
            if (isrequired != null)
                IsRequired = (bool)isrequired;
            IsStandAlone = false;
            if (Dependencies.Count == 0)
                IsStandAlone = true;
            
            // set data type
            switch (valuetype)
            {
                case "System.Int32[]":
                case "System.Int32":
                case "System.Boolean":
                case "System.String":
                    {
                        break;
                    }
                default:
                    throw new ArgumentException($"Type {valuetype} not supported.");
            }
            ValueType = valuetype;
            Type otype = UtilityType.ParseType(valuetype);
            Value = Convert.ChangeType(value, otype);
        }
    }
}
