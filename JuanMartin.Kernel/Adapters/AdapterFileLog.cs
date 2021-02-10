using JuanMartin.Kernel.Messaging;
using JuanMartin.Kernel.RuleEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace JuanMartin.Kernel.Adapters
{
    public class AdapterFileLog : IExchangeRequest
    {
        private StreamWriter _writer;
        private string _fileName;
        private bool _append;
        private bool _writeLine;
        private string _nameFormat;
        private string _lineHeaderFormat;
        private ExpressionEvaluator _formatter;
        Dictionary<string, Symbol> _facts = new Dictionary<string, Symbol>();

        private bool _isConnected;

        //todo: add a custom string formatter

        public AdapterFileLog(ValueHolder Writer)
        {
            _fileName = (string)Writer.Value;

            //Default settings
            _append = true;
            _writeLine = true;
            _nameFormat = "String.Format('" + Writer.Name + "-{0:MM-dd-yyyy}.log', Date.Now())";
            _lineHeaderFormat = "String.Format('[{0:T}]', Date.Now())";

            try
            {
                if (Writer.HasAnnotations())
                {
                    if (Writer.GetAnnotation("AppendMessage") != null)
                        _append = (bool)Writer.GetAnnotation("AppendMessage").Value;
                    if (Writer.GetAnnotation("WriteAlwaysNewLine") != null)
                        _writeLine = (bool)Writer.GetAnnotation("WriteAlwaysNewLine").Value;
                    if (Writer.GetAnnotation("FileNameStringFormat") != null)
                    {
                        _nameFormat = (string)Writer.GetAnnotation("FileNameFormatExpression").Value;

                        ValueHolder facts = Writer.GetAnnotation("FileNameFormatExpression").GetAnnotation("Arguments");
                        if (facts != null)
                            foreach (ValueHolder argument in facts.Annotations)
                            {
                                Symbol fact = new Symbol(argument.Name, new Value(argument.Value, argument.Value.GetType()), Symbol.TokenType.Fact);
                                _facts.Add(facts.Name, fact);
                            }
                    }
                    if (Writer.GetAnnotation("MessageStringFormat") != null)
                    {
                        _lineHeaderFormat = (string)Writer.GetAnnotation("MessageFormatExpression").Value;

                        ValueHolder facts = Writer.GetAnnotation("MessageStringFormat").GetAnnotation("Arguments");
                        if (facts != null)
                            foreach (ValueHolder argument in facts.Annotations)
                            {
                                Symbol fact = new Symbol(argument.Name, new Value(argument.Value, argument.Value.GetType()), Symbol.TokenType.Fact);
                                _facts.Add(facts.Name, fact);
                            }
                    }
                }
            }
            catch
            {
                //Do nothing if some error occurred reading adapter configuration so use Defaults
            }

            _formatter = new ExpressionEvaluator();

            //Build filename if not specified in the Writer value
            if (Path.GetFileName(_fileName) == string.Empty)
            {
                _formatter.Parse(_nameFormat);
                Symbol result = _formatter.Evaluate(_facts);

                _fileName += (string)result.Value.Result;
            }

            _isConnected = Connect();
        }

        public bool Connect()
        {
            try
            {
                if (_writer == null)
                {
                    _writer = new StreamWriter(_fileName, _append);
                    _isConnected = true;
                }
            }
            catch (Exception e)
            {
                _isConnected = false;
                throw new Exception("Error  creating file logger.", e);
            }

            return _isConnected;
        }

        public void Disconnect()
        {
            if (_writer != null)
                _writer.Close();
        }

        public void Send(string Message)
        {
            _formatter.Parse(_lineHeaderFormat);
            Symbol result = _formatter.Evaluate(_facts);
            string header = (string)result.Value.Result;

            Message = string.Concat(header, " ", Message);

            if (_writeLine)
                _writer.WriteLine(Message);
            else
                _writer.Write(Message);
        }

        public void Send(IMessage Message)
        {
            string text = (string)Message.Data.Value;

            Send(text);
        }

        ~AdapterFileLog()
        {
            Disconnect();
        }
    }
}
