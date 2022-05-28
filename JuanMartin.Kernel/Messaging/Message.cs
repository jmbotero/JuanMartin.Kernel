using System;

namespace JuanMartin.Kernel.Messaging
{
    public class Message : IMessage, IRecordSet, ICloneable
    {
        private readonly ValueHolder _payload;

        public Message(string Name, string Type, string Text)
        {
            Value = new ValueHolder(Name);
            Header = new ValueHolder("Header");
            _payload = new ValueHolder("Data");

            AddHeader();

            this.Dtm = DateTime.Now;
            this.Name = Name;
            this.MessageType = Type;
            if (Text != string.Empty)
                this.AddData(Text);

            Value.AddAnnotation(Header);
            Value.AddAnnotation(_payload);
        }

        public Message(string Name, string Type)
            : this(Name, Type, string.Empty)
        {
        }

        public Message(string Text)
            : this(string.Empty, string.Empty, Text)
        {
        }

        public Message()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        public Message(IMessage Message)
        {
            Value = Message.Value;
            Header = Message.Header;
            _payload = Message.Data;
        }

        public ValueHolder Value { get; }

        public ValueHolder Header { get; set; }

        ValueHolder IMessage.Data
        {
            get { return _payload; }
        }

        ValueHolder IRecordSet.Data
        {
            get { return (ValueHolder)_payload.Value; }
        }

        public string MessageType
        {
            get
            {
                try
                {
                    return (string)Header.GetAnnotation("Type").Value;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set { Header.GetAnnotation("Type").Value = value; }
        }

        public string Name
        {
            get
            {
                try
                {
                    return (string)Header.GetAnnotation("Name").Value;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set { Header.GetAnnotation("Name").Value = value; }
        }

        public DateTime Dtm
        {
            get
            {
                try
                {
                    return (DateTime)Header.GetAnnotation("Dtm").Value;
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
            set { Header.GetAnnotation("Dtm").Value = value; }
        }

        private void AddHeader()
        {
            ValueHolder name = new ValueHolder("Name");
            ValueHolder type = new ValueHolder("Type");
            ValueHolder dtm = new ValueHolder("Dtm");

            Header.AddAnnotation(name);
            Header.AddAnnotation(type);
            Header.AddAnnotation(dtm);
        }

        public void AddSender(string Name, string Type, object Value)
        {
            ValueHolder sender = new ValueHolder("Sender");
            ValueHolder name = new ValueHolder("Name", Name);
            ValueHolder type = new ValueHolder("Type", Type);
            sender.Value = Value;

            sender.AddAnnotation(name);
            sender.AddAnnotation(type);

            Header.AddAnnotation(sender);
        }

        public void AddSender(string Name, string Type)
        {
            AddSender(Name, Type, null);
        }

        public void AddReceiver(string Name, string Type, object Value)
        {
            ValueHolder receiver = new ValueHolder("Receiver");
            ValueHolder name = new ValueHolder("Name", Name);
            ValueHolder type = new ValueHolder("Type", Type);
            receiver.Value = Value;

            receiver.AddAnnotation(name);
            receiver.AddAnnotation(type);

            Header.AddAnnotation(receiver);
        }

        public void AddReceiver(string Name, string Type)
        {
            AddReceiver(Name, Type, null);
        }

        public void AddData(object Data)
        {
            Value payload = new Value(Data);

            _payload.ValueContainer = payload;
        }

        #region Clonable interface methods
        public object Clone()
        {
            return new Message(this);
        }
        #endregion
    }
}
