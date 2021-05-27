using System;
using JuanMartin.Kernel.Attributes;

namespace JuanMartin.Kernel.Processors
{
    public class TaskEventArgs : EventArgs
    {
        public enum ExecutionStatus
        {
            [StringValue("INPOGRESS")]
            InProgress,
            [StringValue("RUNNING")]
            Running,
            [StringValue("COMPLETE")]
            Complete,
            [StringValue("ERROR")]
            Error
        };
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
