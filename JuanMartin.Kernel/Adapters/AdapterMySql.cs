using JuanMartin.Kernel.Messaging;
using JuanMartin.Kernel.RuleEngine;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace JuanMartin.Kernel.Adapters
{

    public class AdapterMySql : IExchangeRequestReply
    {
        private string _connectionString;
        private IMessage _response;
        private IMessage _request;
        private ValueHolder _responseData;
        private bool _isConnected;

        #region Private Members
        private void Initialize(string ConnectionString)
        {
            _connectionString = ConnectionString;
            _response = new Message();
            _responseData = new ValueHolder();

            _isConnected = Connect();
        }

        private ValueHolder GetResultSet(MySqlDataReader Reader, string type, string text)
        {
            ValueHolder result = null;
            int rowCount = 1;

            if (Reader.HasRows)
            {
                result = new ValueHolder(type, text);

                while (Reader.Read())
                {
                    ValueHolder row = new ValueHolder("Record", rowCount);

                    for (int i = 0; i < Reader.FieldCount; i++)
                    {
                        ValueHolder field = new ValueHolder(Reader.GetName(i).ToString(), Reader.GetValue(i));

                        row.AddAnnotation(field);
                    }

                    result.AddAnnotation(row);
                    rowCount++;
                }

            }

            return result;
        }

        private ValueHolder ExecuteProcedure(string Name, string Command)
        {
            ExpressionEvaluator expression = new ExpressionEvaluator();
            List<object> parameters = new List<object>();

            expression.Parse(Command);

            List<Symbol> symbols = expression.PostFix;

            string commandName = (string)symbols[0].Name;
            foreach(var s in symbols)
            {
                if (s != null)
                {
                    if (s.Value != null)
                        parameters.Add(s.Value.Result);
                    //else
                        //throw new ArgumentNullException($"{s} value not defined.");
                }
            }

            //Build stored procedure call as query: this is a workaround because otherwise we  need to know the exact param names
            string commandText = string.Format("CALL {0}(", commandName);
            string p = "";
            for (int i = 1; i < parameters.Count + 1; i++)
            {
                var symbol = symbols[i]?.Value;
                if (symbol != null)
                {
                    if (p.Length > 0)
                       p += ",";
                    p += "@p" + i;
                }
            }
            commandText += p + ");";

            //Because of same problem above pass the 'Call' as the command text and do not set the type as sproc
            MySqlCommand command = new MySqlCommand(commandText, Connection);

            //command.CommandType = System.Data.CommandType.StoredProcedure;

            //Add param values so the mysqlclient can replace them in the command text
            for (int i = 1; i < parameters.Count + 1; i++)
            {
                var symbol = symbols[i]?.Value;
                if(symbol != null)
                   command.Parameters.AddWithValue("@p" + i, symbol.Result);
            }

            command.Connection.Open();

            MySqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            ValueHolder results = GetResultSet(reader, Name, Command);

            reader.Close();
            return results;
        }

        private ValueHolder ExecuteQuery(string Name, string Query)
        {
            MySqlCommand command = new MySqlCommand(Query, Connection)
            {
                CommandType = CommandType.Text
            };
            command.Connection.Open();

            MySqlDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

            ValueHolder results = GetResultSet(reader, Name, Query);

            reader.Close();
            return results;
        }
        #endregion

        #region Constructors
        public AdapterMySql(string ConnectionString)
        {
            Initialize(ConnectionString);
        }

        public AdapterMySql(string Server, string Database, string User, string Password)
        {
            Initialize(string.Format("SERVER={0};" +
                "DATABASE={1};" +
                    "UID={2};" +
                "PASSWORD={3};", Server, Database, User, Password));
        }
        #endregion

        ~AdapterMySql()
        {
            Disconnect();
        }

        #region Public Properties
        public MySqlConnection Connection { get; private set; }
        #endregion

        #region Adapter interface methods
        public bool Connect()
        {
            try
            {
                if (Connection == null)
                {
                    Connection = new MySqlConnection(_connectionString);
                    _isConnected = true;
                }
            }
            catch (Exception e)
            {
                _isConnected = false;
                throw new Exception(string.Format("Error connecting to MySql db '{0}'.", _connectionString), e);
            }

            return _isConnected;
        }

        public void Disconnect()
        {
            Connection.Close();
        }

        public void Send(IMessage Request)
        {
            //Initialize response objwects for new IExchangeRequest
            _response = new Message();
            _responseData = new ValueHolder();

            _request = Request;

            ValueHolder request = (ValueHolder)Request.Data.Value;

            string resultsName = request.Name;
            string query = (string)request.Value;

            string type = Request.MessageType;

            CommandType commandType = (CommandType)Enum.Parse(typeof(CommandType), type);

            switch (commandType)
            {
                case CommandType.StoredProcedure:
                    _responseData = ExecuteProcedure(resultsName, query);
                    break;
                case CommandType.Text:
                    _responseData = ExecuteQuery(resultsName, query);
                    break;
                case CommandType.TableDirect:
                    break;
                default:
                    throw new Exception("Only StoredProcedure and Text are mysql adapter valid request types.");
            }
        }

        public IMessage Receive()
        {
            if (_request != null)
            {
                _response.Header = (ValueHolder)_request.Header.Clone();
                _response.Dtm = DateTime.Now;
                _response.AddReceiver("Mysql", this.GetType().ToString());
                _response.AddData(_responseData);
            }

            return _response;
        }

        #endregion
    }
}
