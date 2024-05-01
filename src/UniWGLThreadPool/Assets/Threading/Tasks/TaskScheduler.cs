using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace YoL.WebGL.Threading.Tasks
{
    using SystemTaskScheduler = System.Threading.Tasks.TaskScheduler;

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

        public static SystemTaskScheduler Instance { get; } = new TaskScheduler();

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return this.scheduledTasks;
        }

        protected override void QueueTask(Task task)
        {
            ThreadPool.QueueUserWorkItem(callback, (task, this));
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;
    }
}
