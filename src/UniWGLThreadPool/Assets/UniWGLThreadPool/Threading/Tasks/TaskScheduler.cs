using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace YoL.WebGL.Threading.Tasks
{
    using SystemTaskScheduler = System.Threading.Tasks.TaskScheduler;

    /// <summary>
    /// An implementation of TaskScheduler that uses the <see cref="ThreadPool">ThreadPool</see> scheduler
    /// </summary>
    public class TaskScheduler : SystemTaskScheduler
    {
        private static readonly WaitCallback callback = static s =>
        {
            if (s is (Task task, TaskScheduler @this))
            {
                @this.TryExecuteTask(task);
            }
        };

        private ConcurrentQueue<Task> scheduledTasks = new();

        private TaskScheduler()
        {
            _ = this.Id;
        }

        /// <summary>
        /// Get <see cref="TaskScheduler">TaskScheduler</see> instance.
        /// </summary>
        public static SystemTaskScheduler Instance { get; } = new TaskScheduler();

        /// <inheritdoc/>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return this.scheduledTasks;
        }

        /// <inheritdoc/>
        protected override void QueueTask(Task task)
        {
            ThreadPool.QueueUserWorkItem(callback, (task, this));
        }

        /// <inheritdoc/>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;
    }
}
