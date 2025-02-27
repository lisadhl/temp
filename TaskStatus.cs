using System;

namespace A.TaskDispatching
{
    /// <summary>
    /// 调度任务状态
    /// </summary>
    public enum SchedulerTaskStatus
    {
        /// <summary>
        /// 待機
        /// </summary>
        Waiting,

        /// <summary>
        /// 起動開始
        /// </summary>
        Starting,

        /// <summary>
        /// 起動完了
        /// </summary>
        Started,

        /// <summary>
        /// 正常終了
        /// </summary>
        Succeeded,

        /// <summary>
        /// 異常終了
        /// </summary>
        Failed,

        /// <summary>
        /// 中止
        /// </summary>
        Pending,
    }

    public static class SchedulerTaskStatusExtensions
    {
        /// <summary>
        /// 获取任务状态显示名称。
        /// </summary>
        /// <param name="taskStatus"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetDisplayName(this SchedulerTaskStatus taskStatus)
        {
            switch (taskStatus)
            {
                case SchedulerTaskStatus.Waiting:
                    return "待機";

                case SchedulerTaskStatus.Starting:
                    return "起動開始";

                case SchedulerTaskStatus.Started:
                    return "起動完了";

                case SchedulerTaskStatus.Succeeded:
                    return "正常終了";

                case SchedulerTaskStatus.Failed:
                    return "異常終了";

                case SchedulerTaskStatus.Pending:
                    return "中止";

                default:
                    throw new InvalidOperationException($"Cannot recognize scheduler task status: {taskStatus}");
            };
        }
    }
}
