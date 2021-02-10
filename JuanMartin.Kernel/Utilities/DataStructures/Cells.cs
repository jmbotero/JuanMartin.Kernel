namespace JuanMartin.Kernel.Utilities.DataStructures
{
    public class Cells
    {
        private string[] _values;
        private string _id;
        private int _product;

        public Cells(int numberOfValues)
        {
            _id = string.Empty;
            _values = new string[numberOfValues];
            _product = 0;
        }

        public Cells(string id, int numberOfValues) : this(numberOfValues)
        {
            _id = id;
        }

        public Cells(string id, string[] values)
        {
            _id = id;
            _values = values;
        }

        public Cells(string id, string[] values, int product) : this(id, values)
        {
            _product = product;
        }

        public string[] Values
        {
            get { return _values; }
            set { _values = value; }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int Product
        {
            get { return _product; }
            set { _product = value; }
        }
    }
}
