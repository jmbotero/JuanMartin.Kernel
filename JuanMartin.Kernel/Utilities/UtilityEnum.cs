using JuanMartin.Kernel.Attributes;
using System;
using System.Collections;

namespace JuanMartin.Kernel.Utilities
{
    public class UtilityEnum
    {
        private static Hashtable _stringValues;

        public static string GetStringValue(Enum value)
        {
            string output = null;
            var type = value.GetType();


            //Check first in our cached results...
            if (_stringValues == null)
                _stringValues = new Hashtable();

            if (_stringValues.ContainsKey(value))
                output = (_stringValues[value] as StringValueAttribute).Value;
            else
            {
                //Look for our 'StringValueAttribute' 
                //in the field's custom attributes
                var fi = type.GetField(value.ToString());
                var attrs =
                   fi.GetCustomAttributes(typeof(StringValueAttribute),
                                           false) as StringValueAttribute[];
                if (attrs.Length > 0)
                {
                    _stringValues.Add(value, attrs[0]);
                    output = attrs[0].Value;
                }
            }

            return output;
        }
    }
}
