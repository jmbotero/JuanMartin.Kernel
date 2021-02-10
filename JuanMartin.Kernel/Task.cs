namespace JuanMartin.Kernel
{
    public class Task : ITask
    {
        private ActionOnTaskTimeout _onTimeout;
        private ValueHolder _parameters;
        private string _name;

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

        public virtual void Execute()
        {
            PreExecute(new TaskEventArgs(_parameters, TaskEventArgs.INPROGRESS_EXECUTION_STATUS));

            //Task logic callback
            OnExecute(new TaskEventArgs(_parameters, TaskEventArgs.RUNNING_EXECUTION_STATUS));

            PostExecute(new TaskEventArgs(_parameters, TaskEventArgs.COMPLETE_EXECUTION_STATUS));
        }
    }
}
