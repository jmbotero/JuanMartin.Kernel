using JuanMartin.Kernel.Messaging;

namespace JuanMartin.Kernel.Adapters
{
    public class AdapterMsmq : IExchangeRequestReply
    {
        private IMessage _response;

        public AdapterMsmq()
        {
            _response = new Message();
        }

        public void Send(IMessage Request)
        {
        }

        public IMessage Receive()
        {
            return _response;
        }

        public bool Connect()
        {
            return true;
        }

        public void Disconnect()
        {
        }
    }
}
