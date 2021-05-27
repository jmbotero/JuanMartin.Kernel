namespace JuanMartin.Kernel.Attributes
{
    /// <summary>
    /// <see cref="https://weblogs.asp.net/stefansedich/enum-with-string-values-in-c"/>
    /// </summary>
    public class StringValueAttribute : System.Attribute
    {

        private readonly string _value;

        public StringValueAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

    }
}
