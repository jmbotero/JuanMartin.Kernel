using System;

namespace JuanMartin.Kernel
{
    public class TaskEventArgs : EventArgs
    {
        public const string INPROGRESS_EXECUTION_STATUS = "INPROGRESS";
        public const string RUNNING_EXECUTION_STATUS = "RUNNING";
        public const string ERROR_EXECUTION_STATUS = "ERROR";
        public const string COMPLETE_EXECUTION_STATUS = "COMPLETE";

        private int _executionId;
        private string _status;
        private ValueHolder _parameters;

        public TaskEventArgs(ValueHolder Parameters, string Status)
        {
            this.Parameters = Parameters;
            this.Status = Status;
        }

        public int ExecutionId
        {
            get { return _executionId; }
            set { _executionId = value; }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public ValueHolder Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
    }
}
