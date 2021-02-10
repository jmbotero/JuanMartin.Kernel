namespace JuanMartin.Kernel
{
    public delegate void TaskEventHandler(object sender, TaskEventArgs e);

    public enum ActionOnTaskTimeout { Abort, Skip };

    public interface ITask
    {
        event TaskEventHandler PreTaskHandler;
        event TaskEventHandler TaskHandler;
        event TaskEventHandler PostTaskHandler;

        string Name
        {
            get;
        }

        ValueHolder Parameters
        {
            get;
        }

        ActionOnTaskTimeout OnTimeout
        {
            get;
            set;
        }

        void Execute();
    }
}
