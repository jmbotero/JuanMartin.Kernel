namespace JuanMartin.Kernel.Messaging
{
    public interface IExchangeRequestReply
    {
        void Send(IMessage Request);
        IMessage Receive();
        void Disconnect();
        bool Connect();
    }
}
