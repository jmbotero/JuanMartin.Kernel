using System;
using System.Text;

namespace JuanMartin.Kernel.Formatters
{
    public class ValueHolderFormatter
    {
        private StringBuilder _buffer = new StringBuilder();

        private bool _addNameValuePairs;
        private bool _addNameHeader;
        private char _newItem;
        private char _newLine;
        private string _dateFormatString;

        public ValueHolderFormatter() : this(false, false, "MM/dd/yyyy", '\n', ',') { }

        public ValueHolderFormatter(bool AddHeader) : this(AddHeader, false, "MM/dd/yyyy", '\n', ',') { }

        public ValueHolderFormatter(bool AddHeader, string DateFormatString) : this(AddHeader, false, DateFormatString, '\n', ',') { }

        public ValueHolderFormatter(bool AddHeader, bool AddNameValuePairs, string DateFormatString, char NewLine, char NewItem)
        {
            _addNameHeader = AddHeader;
            _addNameValuePairs = AddNameValuePairs;
            _newItem = NewItem;
            _newLine = NewLine;
            _dateFormatString = DateFormatString;
        }

        public string ToString(ValueHolder Source)
        {
            int counter = 0;
            string header;
            string line;

            foreach (ValueHolder charge in Source.Annotations)
            {
                header = string.Empty;
                line = string.Empty;

                foreach (ValueHolder item in charge.Annotations)
                {
                    if (_addNameHeader && counter == 0)
                    {
                        if (header.Length > 0) { header += _newItem; }

                        header += item.Name;
                    }

                    if (line.Length > 0) { line += _newItem; }

                    if (item.Value.GetType().FullName == "System.DateTime")
                    {
                        line += ((DateTime)item.Value).ToString(_dateFormatString);
                    }
                    else
                    {
                        line += item.Value.ToString();
                    }
                }

                if (header.Length > 0)
                {
                    _buffer.Append(header);
                    _buffer.Append(_newLine);
                }

                if (line.Length > 0)
                {
                    _buffer.Append(line);
                    _buffer.Append(_newLine);
                }

                counter++;
            }

            return _buffer.ToString();
        }
    }
}
