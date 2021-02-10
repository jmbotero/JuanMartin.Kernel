using JuanMartin.Kernel.Messaging;
using System.Diagnostics;

namespace JuanMartin.Kernel.Adapters
{
    public class AdapterEventLog : IExchangeRequest
    {
        private string _source;
        private string _log;
        private EventLogEntryType _type;
        private string _lastEvent;

        private bool _isConnected;

        public AdapterEventLog(ValueHolder Parameters)
        {
            _isConnected = false;
            _source = Parameters.Name;

            //Default settings
            _log = "Application";
            _type = EventLogEntryType.Information;

            _lastEvent = string.Empty;

            if (Parameters.Value is EventLogEntryType)
                _type = (EventLogEntryType)Parameters.Value;

            try
            {
                if (Parameters.HasAnnotations())
                {
                    if (Parameters.GetAnnotation("Log") != null)
                        _log = (string)Parameters.GetAnnotation("Log").Value;

                }
            }
            catch
            {
                //Do nothing if some error occurred reading adapter configuration so use Defaults
            }


            Connect();
        }

        public bool Connect()
        {
            if (!_isConnected)
            {
                if (!EventLog.SourceExists(_source))
                    EventLog.CreateEventSource(_source, _log);

                _isConnected = true;
            }

            return _isConnected;
        }

        public void Disconnect()
        {
        }

        public void Send(IMessage Message)
        {
            try
            {
                if (Message.Data.HasAnnotations())
                {
                    if (Message.Data.GetAnnotation("Type") != null)
                        _type = (EventLogEntryType)Message.Data.GetAnnotation("Type").Value;

                }
            }
            catch
            {
                //Do nothing if some error occurred reading adapter configuration so use Defaults
            }
            string text = (string)Message.Data.Value;

            Send(text, _type);
        }

        public void Send(string Message, EventLogEntryType Type)
        {
            EventLog.WriteEntry(_source, Message, Type);
        }

        public void Send(string Message)
        {
            Send(Message, _type);
        }
    }
}
