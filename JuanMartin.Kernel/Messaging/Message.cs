using System;

namespace JuanMartin.Kernel.Messaging
{
    public class Message : IMessage, IRecordSet, ICloneable
    {
        private ValueHolder _message;
        private ValueHolder _header;
        private ValueHolder _payload;

        public Message(string Name, string Type, string Text)
        {
            _message = new ValueHolder(Name);
            _header = new ValueHolder("Header");
            _payload = new ValueHolder("Data");

            AddHeader();

            this.Dtm = DateTime.Now;
            this.Name = Name;
            this.MessageType = Type;
            if (Text != string.Empty)
                this.AddData(Text);

            _message.AddAnnotation(_header);
            _message.AddAnnotation(_payload);
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
            _message = Message.Value;
            _header = Message.Header;
            _payload = Message.Data;
        }

        public ValueHolder Value
        {
            get { return _message; }
        }

        public ValueHolder Header
        {
            get { return _header; }
            set { _header = value; }
        }

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
                    return (string)_header.GetAnnotation("Type").Value;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set { _header.GetAnnotation("Type").Value = value; }
        }

        public string Name
        {
            get
            {
                try
                {
                    return (string)_header.GetAnnotation("Name").Value;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set { _header.GetAnnotation("Name").Value = value; }
        }

        public DateTime Dtm
        {
            get
            {
                try
                {
                    return (DateTime)_header.GetAnnotation("Dtm").Value;
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
            set { _header.GetAnnotation("Dtm").Value = value; }
        }

        private void AddHeader()
        {
            ValueHolder name = new ValueHolder("Name");
            ValueHolder type = new ValueHolder("Type");
            ValueHolder dtm = new ValueHolder("Dtm");

            _header.AddAnnotation(name);
            _header.AddAnnotation(type);
            _header.AddAnnotation(dtm);
        }

        public void AddSender(string Name, string Type, object Value)
        {
            ValueHolder sender = new ValueHolder("Sender");
            ValueHolder name = new ValueHolder("Name", Name);
            ValueHolder type = new ValueHolder("Type", Type);
            sender.Value = Value;

            sender.AddAnnotation(name);
            sender.AddAnnotation(type);

            _header.AddAnnotation(sender);
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

            _header.AddAnnotation(receiver);
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
