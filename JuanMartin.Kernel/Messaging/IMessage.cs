using System;

namespace JuanMartin.Kernel.Messaging
{
    public interface IMessage
    {
        ValueHolder Value
        {
            get;
        }

        ValueHolder Header
        {
            get;
            set;
        }

        ValueHolder Data
        {
            get;
        }

        string Name
        {
            get;
            set;
        }

        string MessageType
        {
            get;
            set;
        }

        DateTime Dtm
        {
            get;
            set;
        }

        void AddSender(string Name, string Type);
        void AddReceiver(string Name, string Type);
        void AddData(object Data);
        object Clone();
    }
}
