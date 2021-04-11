using JuanMartin.Kernel.Messaging;
using System;
using System.Data;

namespace JuanMartin.Kernel.Test.Adapters
{
    public class AdapterMySqlMock : IExchangeRequestReply
    {
        private readonly IMessage _response;
        private IMessage _request;
        private ValueHolder _responseData;

        private readonly ValueHolder _row;

        public IMessage Request { get => _request; set => _request = value; }

        public bool Connect()
        {
            return true;
        }

        public void Disconnect()
        {
        }

        public AdapterMySqlMock()
        {
            _response = new Message();
            _responseData = new ValueHolder();

            _row = new ValueHolder("Record", 1);

            ValueHolder id = new ValueHolder("id", 1);
            ValueHolder name = new ValueHolder("name", "foo");
            _row.AddAnnotation(id);
            _row.AddAnnotation(name);
        }


        public void Send(IMessage Request)
        {
            _request = Request;

            string query = (string)Request.Data.Value;

            string type = Request.MessageType;

            CommandType commandType = (CommandType)Enum.Parse(typeof(CommandType), type);

            switch (commandType)
            {
                case CommandType.StoredProcedure:
                    _responseData = new ValueHolder("Procedure", query);
                    break;
                case CommandType.Text:
                    _responseData = new ValueHolder("Query", query);
                    break;
                default:
                    throw new Exception("Only StoredProcedure and Text are mysql adapter valid request types.");
            }

            _responseData.AddAnnotation(_row);
        }

        public IMessage Receive()
        {
            _response.Header = (ValueHolder)_response.Header.Clone();
            _response.Dtm = DateTime.Now;
            _response.AddReceiver("Mysql", this.GetType().ToString());
            _response.AddData(_responseData);

            return _response;
        }
    }
}
