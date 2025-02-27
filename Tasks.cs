using A.UI.Service;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace A.TaskDispatching
{
    /// <summary>
    /// 工作任务，用于具体的任务，比如起动进程。
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTimeOffset CreationTime { get; }

        /// <summary>
        /// 执行任务
        /// </summary>
        void Execute();

        /// <summary>
        /// 异步执行任务。
        /// </summary>
        /// <returns></returns>
        Task ExecuteAsync();

        /// <summary>
        /// 启动事件
        /// </summary>
        event EventHandler<TaskStartingEventArgs> Starting;

        /// <summary>
        /// 状态报告事件
        /// </summary>
        event EventHandler<TaskReportStatusEventArgs> ReportStatus;

        /// <summary>
        /// 任务完成事件
        /// </summary>
        event EventHandler<TaskCompletedEventArgs> Completed;
    }

    public abstract class TaskBase : ITask
    {
        public abstract string Name { get; }

        public DateTimeOffset CreationTime { get; set; } = MinashiDateTime.Now;

        public event EventHandler<TaskStartingEventArgs> Starting;
        public event EventHandler<TaskReportStatusEventArgs> ReportStatus;
        public event EventHandler<TaskCompletedEventArgs> Completed;

        public abstract void Execute();

        public Task ExecuteAsync()
        {
            return Task.Factory.StartNew(() => Execute());
        }

        protected virtual void OnStarting(TaskStartingEventArgs e)
        {
            Starting?.Invoke(this, e);
        }

        protected virtual void OnReportStatus(TaskReportStatusEventArgs e)
        {
            ReportStatus?.Invoke(this, e);
        }

        protected virtual void OnCompleted(TaskCompletedEventArgs e)
        {
            Completed?.Invoke(this, e);
        }
    }

    public static class TaskExtensions
    {
        /// <summary>
        /// 对工作任务安排调度。
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="number"></param>
        /// <param name="runNextOnFailed"></param>
        /// <returns></returns>
        public static PrimitiveSchedulerTask ArrangeScheduler(
                this ITask worker,
                int number,
                bool runNextOnFailed,
                TaskItem configuration
            )
        {
            return new PrimitiveSchedulerTask(
                    worker,
                    number,
                    runNextOnFailed,
                    configuration
                );
        }
    }

    /// <summary>
    /// 用于执行工作任务。
    /// </summary>
    public abstract class SchedulerTask
    {
        protected SchedulerTask()
        {
        }

        /// <summary>
        /// 任务名称
        /// </summary>
        public virtual string Name => string.Empty;

        /// <summary>
        /// 任务编号
        /// </summary>
        public virtual int Number { get; }

        /// <summary>
        /// 配置项
        /// </summary>
        public TaskItem Configuration { get; protected set; }

        /// <summary>
        /// 获取任务状态。
        /// </summary>
        public abstract SchedulerTaskStatus Status { get; }

        /// <summary>
        /// 用于决定当本条任务失败时，下条任务是否执行（只有当下条任务等待时才有效）。
        /// </summary>
        public abstract bool RunNextOnFailed { get; }

        /// <summary>
        /// 异步执行任务
        /// </summary>
        /// <param name="remainderTaskCollector"></param>
        /// <returns></returns>
        public abstract Task ExecuteAsync(List<Task> remainderTaskCollector);

        /// <summary>
        /// 中上任务。
        /// </summary>
        /// <returns></returns>
        public abstract bool Pending();

        /// <summary>
        /// 用于确定是下条任务可否执行。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool CanRunNext()
        {
            if (!IsCompleted)
            {
                throw new InvalidOperationException($"任务 {Name} 尚未完成，无法判定下条任务可否执行。");
            }

            if (Status == SchedulerTaskStatus.Pending)
            {
                return false;
            }

            return Status == SchedulerTaskStatus.Succeeded || RunNextOnFailed;
        }

        /// <summary>
        /// 确定任务是否完成。
        /// </summary>
        public bool IsCompleted => Status == SchedulerTaskStatus.Succeeded ||
                                   Status == SchedulerTaskStatus.Failed ||
                                   Status == SchedulerTaskStatus.Pending;
    }

    /// <summary>
    /// 单一调度任务，负责一条工作任务的执行。
    /// </summary>
    public class PrimitiveSchedulerTask : SchedulerTask
    {
        public PrimitiveSchedulerTask(ITask task, int number, bool runNextOnFailed, TaskItem configuration)
        {
            Worker = task;
            CreationTime = task.CreationTime;

            Configuration = configuration;

            Number = number;
            RunNextOnFailed = runNextOnFailed;

            task.Starting += OnWorkerStarting;
            task.ReportStatus += OnTaskReportStatus;
            task.Completed += OnWorkerCompleted;
        }

        /// <summary>
        /// 任务状态变化事件
        /// </summary>
        public event EventHandler<SchedulerTaskStatusChangedEventArgs> TaskStatusChanged;

        /// <summary>
        /// 任务进度报告事件
        /// </summary>
        public event EventHandler<SchedulerTaskProgressReportedEventArgs> TaskProgressReported;

        /// <summary>
        /// 工作任务，用于执行具体的任务，比如起动进程。
        /// </summary>
        public ITask Worker { get; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public override string Name => Worker.Name;

        /// <summary>
        /// 任务编号
        /// </summary>
        public override int Number { get; }

        /// <summary>
        /// 工作任务创建时间
        /// </summary>
        public DateTimeOffset CreationTime { get; }

        /// <summary>
        /// 延迟
        /// </summary>
        public TimeSpan Delay { get; set; }

        private SchedulerTaskStatus _status;

        /// <summary>
        /// 任务状态
        /// </summary>
        public override SchedulerTaskStatus Status
        {
            get { return _status; }
        }

        /// <summary>
        /// 任务执行日志
        /// </summary>
        public IList<string> Log { get; } = new List<string>();

        /// <summary>
        /// 错误
        /// </summary>
        public Exception Error { get; private set; }

        private string _errorMessage;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage => _errorMessage ?? Error?.ToString();

        /// <summary>
        /// 当本条任务失败时，是否执行下条任务。
        /// </summary>
        public override bool RunNextOnFailed { get; }

        /// <summary>
        /// 异步执行任务。
        /// </summary>
        /// <param name="remainderTaskCollector"></param>
        /// <returns></returns>
        public override Task ExecuteAsync(List<Task> remainderTaskCollector) => ExecuteAsync();

        /// <summary>
        /// 异步执行任务。
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteAsync()
        {
            if (Delay > TimeSpan.Zero)
            {
                await Task.Delay(Delay);
            }

            try
            {
                await Worker.ExecuteAsync();
            }
            catch (Exception ex)
            {
                DateTimeOffset timestamp = MinashiDateTime.Now;
                Error = ex;

                ChangeStatus(SchedulerTaskStatus.Failed, timestamp);
            }
        }

        /// <summary>
        /// 中上任务。
        /// </summary>
        /// <returns></returns>
        public override bool Pending()
        {
            if (Status != SchedulerTaskStatus.Waiting)
            {
                return false;
            }

            ChangeStatus(SchedulerTaskStatus.Pending);

            return true;
        }

        public override string ToString() => Name;

        // WorkerTask 事件传递到外部
        private void OnWorkerStarting(object sender, TaskStartingEventArgs e)
        {
            Log.Add(BuildLog(e.Timestamp, "Starting..."));
            ChangeStatus(SchedulerTaskStatus.Running, e.Timestamp);
        }

        private void OnTaskReportStatus(object sender, TaskReportStatusEventArgs e)
        {
            Log.Add(BuildLog(e.Timestamp, e.Message));
            TaskProgressReported?.Invoke(this, new SchedulerTaskProgressReportedEventArgs(e.Timestamp, this, e.Message));
        }

        private void OnWorkerCompleted(object sender, TaskCompletedEventArgs e)
        {
            _errorMessage = e.ErrorMessage;

            Log.Add(
                    BuildLog(
                            e.Timestamp,
                            e.Success ? "Succeeded" : $"Failed: {e.ErrorMessage}"
                        )
                );
            ChangeStatus(e.Success ? SchedulerTaskStatus.Succeeded : SchedulerTaskStatus.Failed, e.Timestamp);
        }

        private void ChangeStatus(SchedulerTaskStatus status, DateTimeOffset timestamp = default)
        {
            if (timestamp == default)
            {
                timestamp = DateTime.Now;
            }

            _status = status;

            TaskStatusChanged?.Invoke(this, new SchedulerTaskStatusChangedEventArgs(timestamp, this));
        }

        private static string BuildLog(DateTimeOffset timestamp, string message) =>
            $"{(timestamp.LocalDateTime):HH:mm:ss} {message}";
    }

    /// <summary>
    /// 复合任务调度器，用于对任务进行分组。
    /// </summary>
    public abstract class CompositeSchedulerTask : SchedulerTask
    {
        public CompositeSchedulerTask(IList<PrimitiveSchedulerTask> schedulerTasks)
        {
            if (schedulerTasks.Count == 0)
            {
                throw new ArgumentException(
                        message: "至少应包含1个任务。",
                        paramName: nameof(schedulerTasks)
                    );
            }

            PrimitiveSchedulerTasks = schedulerTasks;
        }

        /// <summary>
        /// 单一任务调度器列表。
        /// </summary>
        public IList<PrimitiveSchedulerTask> PrimitiveSchedulerTasks { get; }

        public override string ToString() => string.Join(", ", PrimitiveSchedulerTasks);
    }

    /// <summary>
    /// 并行调度任务，里面包含可并行执行的多个单一任务。
    /// </summary>
    /// <param name="primitiveSchedulerTasks"></param>
    public sealed class ParallelCompositeSchedulerTask
        : CompositeSchedulerTask
    {
        public ParallelCompositeSchedulerTask(IList<PrimitiveSchedulerTask> primitiveSchedulerTasks)
            : base(primitiveSchedulerTasks)
        {
        }

        /// <summary>
        /// 任务状态，取最后一个单一任务的状态。
        /// </summary>
        public override SchedulerTaskStatus Status => LastSchedulerTask.Status;

        /// <summary>
        /// 当本条任务失败时，可否执行下条任务，由组中的最后一条任务来决定。
        /// </summary>
        public override bool RunNextOnFailed => LastSchedulerTask.RunNextOnFailed;

        /// <summary>
        /// 异步执行任务，返回最后一条任务的等待对象。(其他任务的等待对象放到参数 remainderTaskCollector)
        /// </summary>
        /// <param name="remainderTaskCollector"></param>
        /// <returns></returns>
        public override Task ExecuteAsync(List<Task> remainderTaskCollector)
        {
            List<Task> all = PrimitiveSchedulerTasks.Select(t => t.ExecuteAsync()).ToList();

            remainderTaskCollector.AddRange(all.Where((_, index) => index < all.Count - 1));

            return all[all.Count - 1];
        }

        /// <summary>
        /// 通知中止本条任务的执行。
        /// </summary>
        /// <returns></returns>
        public override bool Pending()
        {
            bool hasAtOneSet = false;

            foreach (SchedulerTask schedulerTask in PrimitiveSchedulerTasks)
            {
                bool result = schedulerTask.Pending();

                if (result)
                {
                    hasAtOneSet = true;
                }
            }

            return hasAtOneSet;
        }

        public SchedulerTask LastSchedulerTask => PrimitiveSchedulerTasks[PrimitiveSchedulerTasks.Count - 1];
    }

    /// <summary>
    /// 任务总调度器(对应于配置文件)
    /// </summary>
    public sealed class TaskDispatcher
    {
        public TaskDispatcher(IList<SchedulerTask> taskQueue, IList<PrimitiveSchedulerTask> primitiveTasks)
        {
            TaskQueue = taskQueue;
            PrimitiveTasks = primitiveTasks;

            foreach (PrimitiveSchedulerTask task in PrimitiveTasks)
            {
                task.TaskStatusChanged += OnTaskStatusChanged;
                task.TaskProgressReported += OnTaskProgressReported;
            }
        }

        /// <summary>
        /// 调度任务对列。
        /// </summary>
        public IList<SchedulerTask> TaskQueue { get; }

        /// <summary>
        /// 单一调度任务列表。
        /// </summary>
        public IList<PrimitiveSchedulerTask> PrimitiveTasks { get; }

        /// <summary>
        /// 调度任务状态变化事件
        /// </summary>
        public event EventHandler<SchedulerTaskStatusChangedEventArgs> TaskStatusChanged;

        /// <summary>
        /// 任务进度报告事件
        /// </summary>
        public event EventHandler<SchedulerTaskProgressReportedEventArgs> TaskProgressReported;

        /// <summary>
        /// 任务完成事件
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// 异步执行任务列表。
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteAsync()
        {
            List<Task> allRemainderTasks = new List<Task>();
            bool canRunNext = true;

            foreach (SchedulerTask schedulerTask in TaskQueue)
            {
                if (!canRunNext)
                {
                    schedulerTask.Pending();

                    continue;
                }

                await schedulerTask.ExecuteAsync(allRemainderTasks);
                canRunNext = schedulerTask.CanRunNext();
            }

            await Task.WhenAll(allRemainderTasks);

            Completed?.Invoke(this, EventArgs.Empty);
        }

        private void OnTaskStatusChanged(object sender, SchedulerTaskStatusChangedEventArgs e)
        {
            TaskStatusChanged?.Invoke(this, e);
        }

        private void OnTaskProgressReported(object sender, SchedulerTaskProgressReportedEventArgs e)
        {
            TaskProgressReported?.Invoke(this, e);
        }
    }

    public static class ReportBuilder
    {
        public static string Build(TaskDispatcher dispatcher, string config)
        {
            try
            {
                StringWriter writer = new StringWriter();

                writer.WriteLine();
                GenerateTestResults(dispatcher, config, writer);

                return writer.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private static void GenerateTestResults(TaskDispatcher Dispatcher, string Config, TextWriter writer)
        {
            string ruler = string.Empty.PadRight(80, '-');

            writer.WriteLine(ruler);
            writer.WriteLine(Config);

            OutputSchedulerTasks(Dispatcher, writer);

            writer.WriteLine(ruler);
            writer.WriteLine();
        }

        private static void OutputSchedulerTasks(TaskDispatcher dispatcher, TextWriter writer)
        {
            OutputSummary(dispatcher, writer);
            OutputDetail(dispatcher, writer);
        }

        private static void OutputSummary(TaskDispatcher dispatcher, TextWriter writer)
        {
            IList<string> log;
            string ruler = string.Empty.PadRight(140, '-');

            writer.WriteLine(ruler);

            foreach (CompositeSchedulerTask schedulerTask in dispatcher.TaskQueue.Cast<CompositeSchedulerTask>())
            {
                for (int i = 0; i < schedulerTask.PrimitiveSchedulerTasks.Count; i++)
                {
                    if (i != 0)
                    {
                        writer.Write("  ");
                    }

                    PrimitiveSchedulerTask primitive = schedulerTask.PrimitiveSchedulerTasks[i];
                    log = primitive.Log;

                    writer.Write(
                            "{0}. {1} {2:HH:mm:ss} created. {3}",
                            primitive.Number,
                            primitive.Name,
                            primitive.CreationTime,
                            primitive.Status.GetDisplayName()
                        );

                    if (log.Count == 0)
                    {
                        continue;
                    }

                    if (log.Count >= 1)
                    {
                        writer.Write(" ({0} - ", primitive.Log[0].Substring(0, 8));
                    }

                    if (log.Count >= 2)
                    {
                        writer.Write("{0})", primitive.Log[primitive.Log.Count - 1].Substring(0, 8));
                    }
                    else if (log.Count >= 1)
                    {
                        writer.Write(")");
                    }
                }

                writer.WriteLine();
            }

            writer.WriteLine(ruler);
            writer.WriteLine();
        }

        private static void OutputDetail(TaskDispatcher dispatcher, TextWriter writer)
        {
            string ruler = string.Empty.PadRight(30, '-');

            foreach (PrimitiveSchedulerTask schedulerTask in dispatcher.PrimitiveTasks)
            {
                writer.WriteLine(
                        "{0}. {1} {2}",
                        schedulerTask.Number,
                        schedulerTask.Name,
                        schedulerTask.Status.GetDisplayName()
                    );
                writer.WriteLine(ruler);

                foreach (string log in schedulerTask.Log)
                {
                    writer.WriteLine(log);
                }

                if (schedulerTask.Log.Count != 0)
                {
                    writer.WriteLine(ruler);
                }

                writer.WriteLine();
            }
        }
    }
}
