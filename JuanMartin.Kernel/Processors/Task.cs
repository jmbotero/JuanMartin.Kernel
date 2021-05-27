namespace JuanMartin.Kernel.Processors
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class Task : ITask
    {
        private ActionOnTaskTimeout _onTimeout;
        private readonly ValueHolder _parameters;
        private readonly string _name;

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

        public Task(ValueHolder Definition)
        {
            _name = Definition.Name;
            _parameters = Definition;
            _onTimeout = ActionOnTaskTimeout.Skip;
        }

        public string Name
        {
            get { return _name; }
        }

        public ValueHolder Parameters
        {
            get { return _parameters; }
        }

        public ActionOnTaskTimeout OnTimeout
        {
            get { return _onTimeout; }
            set { _onTimeout = value; }
        }

        public virtual void OnExecute(TaskEventArgs e)
        {
            _handler?.Invoke(this, e);
        }

        public virtual void PreExecute(TaskEventArgs e)
        {
            _preHandler?.Invoke(this, e);
        }

        public virtual void PostExecute(TaskEventArgs e)
        {
            _postHandler?.Invoke(this, e);
        }

        public virtual void Execute()
        {
            PreExecute(new TaskEventArgs(_parameters, TaskEventArgs.ExecutionStatus.InProgress.ToString()));

            //Task logic callback
            OnExecute(new TaskEventArgs(_parameters, TaskEventArgs.ExecutionStatus.Running.ToString()));

            PostExecute(new TaskEventArgs(_parameters, TaskEventArgs.ExecutionStatus.Complete.ToString()));
        }
    }
}
