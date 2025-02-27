using System;

namespace A.TaskDispatching
{
    /// <summary>
    /// 工作任务事件参数
    /// </summary>
    /// <param name="timestamp"></param>
    public class TaskEventArgs : EventArgs
    {
        public TaskEventArgs(DateTimeOffset timestamp)
        {
            Timestamp = timestamp;
        }

        public DateTimeOffset Timestamp { get; }
    }

    /// <summary>
    /// 工作任务状态报告事件参数
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="message"></param>
    public sealed class TaskReportStatusEventArgs : TaskEventArgs
    {
        public TaskReportStatusEventArgs(DateTimeOffset timestamp, string message) : base(timestamp)
        {
            Message = message;
        }

        public string Message { get; }
    }

    /// <summary>
    /// 工作任务完成事件参数。
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="errorMessage"></param>
    public sealed class TaskCompletedEventArgs : TaskEventArgs
    {
        public TaskCompletedEventArgs(DateTimeOffset timestamp, string errorMessage = null)
            : base(timestamp)
        {
            ErrorMessage = errorMessage;
        }

        public bool Success => string.IsNullOrEmpty(ErrorMessage);

        public string ErrorMessage { get; }
    }

    /// <summary>
    /// 调度任务事件参数
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="task"></param>
    public abstract class SchedulerTaskEventArgs : TaskEventArgs
    {
        public SchedulerTaskEventArgs(DateTimeOffset timestamp, SchedulerTask task)
            : base(timestamp)
        {
            Task = task;
        }

        public SchedulerTask Task { get; }
    }

    /// <summary>
    /// 调度任务进度报告事件参数
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="task"></param>
    /// <param name="message"></param>
    public sealed class SchedulerTaskProgressReportedEventArgs
        : SchedulerTaskEventArgs
    {
        public SchedulerTaskProgressReportedEventArgs(DateTimeOffset timestamp, SchedulerTask task, string message)
            : base(timestamp, task)
        {
            Message = message;
        }

        public string Message { get; }
    }

    /// <summary>
    /// 调度任务状态变化事件参数
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="task"></param>
    public sealed class SchedulerTaskStatusChangedEventArgs
        : SchedulerTaskEventArgs
    {
        public SchedulerTaskStatusChangedEventArgs(DateTimeOffset timestamp, SchedulerTask task)
            : base(timestamp, task)
        {
        }

        public SchedulerTaskStatus NewStatus => Task.Status;
    }
}
