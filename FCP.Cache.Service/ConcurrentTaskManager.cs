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
                task.ContinueWith((t) => { TryRemove(key); });
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
            private Func<Task> _taskFunc;

            private object _taskMutex;
            private Task _task;

            public ConcurrentTask(Func<Task> taskFunc)
            {
                _taskFunc = taskFunc;

                _taskMutex = new object();
                _task = null;
            }

            public Task Task { get { return GetTask(); } }

            private Task GetTask()
            {
                lock(_taskMutex)
                {
                    if (_task == null && _taskFunc != null)
                    {
                        _task = _taskFunc();
                    }                    
                }

                return _task;
            }
        }
    }
}
