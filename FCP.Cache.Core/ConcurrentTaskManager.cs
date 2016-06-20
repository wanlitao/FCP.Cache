using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FCP.Cache
{
    public class ConcurrentTaskManager
    {
        private readonly ConcurrentDictionary<string, ConcurrentTask> _taskDict = new ConcurrentDictionary<string, ConcurrentTask>();

        public TResult GetTaskResult<TResult>(string key, Func<Task<TResult>> taskFunc)
        {
            var concurrentTask = GetOrAddConcurrentTask(key, taskFunc);

            return concurrentTask.GetInnerTaskResult<TResult>();
        }

        public async Task<TResult> GetTaskResultAsync<TResult>(string key, Func<Task<TResult>> taskFunc)
        {
            var concurrentTask = GetOrAddConcurrentTask(key, taskFunc);

            return await concurrentTask.GetInnerTaskResultAsync<TResult>().ConfigureAwait(false);
        }

        private ConcurrentTask GetOrAddConcurrentTask(string key, Func<Task> taskFunc)
        {
            Func<Task> newTaskFunc = () =>
            {
                var task = taskFunc();
                task.ContinueWith((t) => { TryRemove(key); });
                return task;
            };

            return _taskDict.GetOrAdd(key, new ConcurrentTask(newTaskFunc));
        }

        public bool TryRemove(string key)
        {
            ConcurrentTask concurrentTask;
            return _taskDict.TryRemove(key, out concurrentTask);
        }

        private sealed class ConcurrentTask
        {
            private Func<Task> _taskFunc;
           
            private SemaphoreSlim semaphoreSlim;

            private Task _task;

            public ConcurrentTask(Func<Task> taskFunc)
            {
                _taskFunc = taskFunc;

                semaphoreSlim = new SemaphoreSlim(1);
                _task = null;
            }

            public TResult GetInnerTaskResult<TResult>()
            {
                semaphoreSlim.Wait();
                try
                {
                    if (_task == null && _taskFunc != null)
                    {
                        _task = _taskFunc();
                        _task.Wait();
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }                

                return GetCompleteTaskResult<TResult>();
            }

            public async Task<TResult> GetInnerTaskResultAsync<TResult>()
            {                
                await semaphoreSlim.WaitAsync();
                try
                {
                    if (_task == null && _taskFunc != null)
                    {
                        _task = _taskFunc();
                        await _task.ConfigureAwait(false);
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }

                return GetCompleteTaskResult<TResult>();
            }

            private TResult GetCompleteTaskResult<TResult>()
            {
                var resultTask = _task as Task<TResult>;
                if (resultTask == null)
                    return default(TResult);

                return resultTask.Result;
            }
        }
    }
}
