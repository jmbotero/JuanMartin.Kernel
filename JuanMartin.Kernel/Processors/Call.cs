using JuanMartin.Kernel.RuleEngine;
using System.Diagnostics;

namespace JuanMartin.Kernel.Processors
{
    public class Call : ITask, IAction
    {
        private ActionOnTaskTimeout _onTimeout;
        private ValueHolder _parameters;
        private string _name;
        private ProcessWindowStyle _style;

        private Process _process;

        private event TaskEventHandler _handler;
        event TaskEventHandler ITask.TaskHandler
        {
            add
            {
                if (_handler != null)
                {
                    lock (_handler)
                    {
                        _handler += value;
                    }
                }
                else
                {
                    _handler = new TaskEventHandler(value);
                }
            }
            remove
            {
                if (_handler != null)
                {
                    lock (_handler)
                    {
                        _handler -= value;
                    }
                }
            }
        }

        private event TaskEventHandler _preHandler;
        event TaskEventHandler ITask.PreTaskHandler
        {
            add
            {
                if (_preHandler != null)
                {
                    lock (_preHandler)
                    {
                        _preHandler += value;
                    }
                }
                else
                {
                    _preHandler = new TaskEventHandler(value);
                }
            }
            remove
            {
                if (_preHandler != null)
                {
                    lock (_preHandler)
                    {
                        _preHandler -= value;
                    }
                }
            }
        }

        private event TaskEventHandler _postHandler;
        event TaskEventHandler ITask.PostTaskHandler
        {
            add
            {
                if (_postHandler != null)
                {
                    lock (_postHandler)
                    {
                        _postHandler += value;
                    }
                }
                else
                {
                    _postHandler = new TaskEventHandler(value);
                }
            }
            remove
            {
                if (_postHandler != null)
                {
                    lock (_postHandler)
                    {
                        _postHandler -= value;
                    }
                }
            }
        }

        public Call(ValueHolder Definition)
        {
            string arguments = string.Empty;

            _name = Definition.Name;
            _parameters = Definition;
            _onTimeout = ActionOnTaskTimeout.Skip;
            _style = ProcessWindowStyle.Hidden;

            _process = new Process();

            //Set process information
            try
            {
                if (Definition.HasAnnotations())
                {
                    if (Definition.GetAnnotation("ArgumentString") != null)
                        arguments = (string)Definition.GetAnnotation("ArgumentString").Value;
                    if (Definition.GetAnnotation("WindowStyle") != null)
                        _style = (ProcessWindowStyle)Definition.GetAnnotation("WindowStyle").Value;
                }
            }
            catch
            {
                //Do nothing if some error occurred reading adapter configuration so use Defaults
            }

            _process.StartInfo.FileName = _name;
            _process.StartInfo.Arguments = arguments;
            _process.StartInfo.WindowStyle = _style;
        }

        void ITask.Execute()
        {
            _process.Start();
        }

        Symbol IAction.Execute()
        {
            _process.Start();

            return new Symbol("id", _process.Id, Symbol.TokenType.Value);
        }

        public string Name
        {
            get { return _name; }
        }

        public ValueHolder Parameters
        {
            get { return _parameters; }
        }

        public Process Process
        {
            get { return _process; }
        }

        public ActionOnTaskTimeout OnTimeout
        {
            get { return _onTimeout; }
            set { _onTimeout = value; }
        }

        public virtual void OnExecute(TaskEventArgs e)
        {
            if (_handler != null)
                _handler(this, e);
        }

        public virtual void PreExecute(TaskEventArgs e)
        {
            if (_preHandler != null)
                _preHandler(this, e);
        }

        public virtual void PostExecute(TaskEventArgs e)
        {
            if (_postHandler != null)
                _postHandler(this, e);
        }

        ~Call()
        {
            if (!_process.HasExited)
            {
                if (_process.Responding)
                    _process.CloseMainWindow();
                else
                    _process.Kill();
            }

            _process.Close();
        }
    }
}
