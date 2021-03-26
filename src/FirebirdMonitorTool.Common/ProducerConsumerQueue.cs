using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FirebirdMonitorTool.Common
{
    public class ProducerConsumerQueue : IDisposable
    {
        private class WorkItem
        {
            public readonly TaskCompletionSource<object> TaskSource;
            public readonly Action Action;
            public readonly CancellationToken? CancelationToken;

            public WorkItem(TaskCompletionSource<object> taskSource, Action action, CancellationToken? cancelationToken)
            {
                TaskSource = taskSource;
                Action = action;
                CancelationToken = cancelationToken;
            }
        }

        private BlockingCollection<WorkItem> m_WorkItems = new BlockingCollection<WorkItem>(new ConcurrentQueue<WorkItem>());
        private Task[] m_Workers;

        public ProducerConsumerQueue(int workerCount)
        {
            workerCount = Math.Max(1, workerCount);

            m_Workers = new Task[workerCount];
            for (var i = 0; i < workerCount; i++)
            {
                m_Workers[i] = Task.Factory.StartNew(Consume, TaskCreationOptions.LongRunning);
            }
        }

        public Task Enqueue(Action action, CancellationToken? cancelationToken = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var tcs = new TaskCompletionSource<object>();
            m_WorkItems.Add(new WorkItem(tcs, action, cancelationToken));
            return tcs.Task;
        }

        public void Dispose(TimeSpan? waitOnWorkersTimeout)
        {
            if (m_WorkItems != null)
            {
                m_WorkItems.CompleteAdding();
            }
            if (waitOnWorkersTimeout.HasValue)
            {
                if (m_Workers != null)
                {
                    Task.WaitAll(m_Workers, waitOnWorkersTimeout.Value);
                }
            }
            m_WorkItems = null;
            m_Workers = null;
        }

        private void Consume()
        {
            var items = m_WorkItems.GetConsumingEnumerable();
            foreach (var workItem in items)
            {
                if (workItem.CancelationToken.HasValue
                    && workItem.CancelationToken.Value.IsCancellationRequested)
                {
                    workItem.TaskSource.SetCanceled();
                }
                else
                {
                    // do action an catch exception
                    try
                    {
                        workItem.Action();
                        workItem.TaskSource.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        workItem.TaskSource.SetException(e);
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(TimeSpan.MaxValue);
        }
    }
}
