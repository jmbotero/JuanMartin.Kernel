using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace JuanMartin.Kernel.Processors
{
    public class ProcessorTask
    {
        private readonly int _timeout;
        private bool _currentTaskComplete;
        private List<ValueHolder>.Enumerator _tasks;
        private DateTime _currentTaskBeginExecution;

        public ProcessorTask(ValueHolder TaskList, int Timeout)
        {
            _currentTaskComplete = false;
            _tasks = TaskList.Annotations.GetEnumerator();
            _timeout = Timeout;
        }

        public void Execute()
        {
            while (_tasks.MoveNext())
            {
                ValueHolder definition = _tasks.Current;
                _currentTaskBeginExecution = DateTime.Now;

                //Create new task from valueholder definition
                ITask task = (ITask)definition.Value;
                ((ITask)task).PreTaskHandler += new TaskEventHandler(ProcesssTaskEvent);
                ((ITask)task).PostTaskHandler += new TaskEventHandler(ProcesssTaskEvent);

                //Run task in its own thread
                Thread thread = new Thread(new ThreadStart(task.Execute));

                thread.Start();

                while (!_currentTaskComplete && DateTime.Now.Subtract(_currentTaskBeginExecution).Milliseconds < _timeout)
                {
                    //Wait until timeout is reached or if task is signalled as completed
                }

                //If done waiting and task is not complete, if task is set to abort on timeout then terminate thread
                if (_currentTaskComplete)
                {
                    // commented thread.Abort becaacause is not supported, by 03/01/2021, in .Net core
                    //thread.Abort(); //Request thread to be terminated
                    thread.Join(); //Wait until thread is finished
                }
                else if (!_currentTaskComplete && task.OnTimeout == ActionOnTaskTimeout.Abort)
                {
                    //thread.Abort(); //Request thread to be terminated
                    thread.Join(); //Wait until thread is finished
                }
            }
        }

        private void ProcesssTaskEvent(Object sender, TaskEventArgs e)
        {
            if (e.Status == TaskEventArgs.ExecutionStatus.Complete.ToString())
            {
                _currentTaskComplete = true;
            }
        }
    }
}
