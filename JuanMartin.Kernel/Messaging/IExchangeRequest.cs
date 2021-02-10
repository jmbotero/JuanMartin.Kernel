namespace JuanMartin.Kernel.Messaging
{
    public interface IExchangeRequest
    {
        void Disconnect();
        bool Connect();
        void Send(IMessage Request);
        void Send(string Request);
    }
}
