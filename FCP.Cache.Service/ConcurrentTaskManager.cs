using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FCP.Cache.Service
{
    public class ConcurrentTaskManager
    {
        private readonly ConcurrentDictionary<string, ConcurrentTask> _taskDict = new ConcurrentDictionary<string, ConcurrentTask>();

        public Task GetOrAdd(string key, Func<Task> taskFunc)
        {
            Func<Task> newTaskFunc = () =>
            {
                var task = taskFunc();
                task = task.ContinueWith((t) => { TryRemove(key); });
                return task;
            };
            
            var concurrentTask = _taskDict.GetOrAdd(key, new ConcurrentTask(newTaskFunc));
            return concurrentTask.Task;
        }

        public bool TryRemove(string key)
        {
            ConcurrentTask concurrentTask;
            return _taskDict.TryRemove(key, out concurrentTask);
        }

        private sealed class ConcurrentTask
        {
            private int _taskMutex = 0;           
            private Func<Task> _taskFunc;

            public ConcurrentTask(Func<Task> taskFunc)
            {
                _taskFunc = taskFunc;
            }

            public Task Task { get { return GetTask(); } }

            private Task GetTask()
            {
                if (Interlocked.CompareExchange(ref _taskMutex, 1, 0) == 0)
                {
                    Task task = _taskFunc == null ? null : _taskFunc();
                    _taskFunc = () => { return task; };
                }

                return _taskFunc();
            }
        }
    }
}
