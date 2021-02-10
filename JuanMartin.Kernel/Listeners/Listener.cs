using System;
using System.Timers;

namespace JuanMartin.Kernel.Listeners
{
    public class Listener
    {
        private ITask _task;

        public Listener()
        {
        }

        public void Listen(int Interval, ITask Task)
        {
            Timer _listener = new Timer();

            _task = Task;
            _listener.Elapsed += new ElapsedEventHandler(ProcessTimeEvent);
            _listener.Interval = Interval;

            _listener.Start();

            while (true)
            {
                ; //do nothing
            }
        }

        public virtual void ProcessTimeEvent(Object sender, ElapsedEventArgs e)
        {
            _task.TaskHandler += new TaskEventHandler(ProcessTaskEvent);
            _task.Execute();
        }

        public virtual void ProcessTaskEvent(Object sender, TaskEventArgs e)
        {
            Console.WriteLine(e.Status);
        }
    }
}
