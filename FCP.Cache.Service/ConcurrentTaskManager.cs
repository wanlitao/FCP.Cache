using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FCP.Cache.Service
{
    public class ConcurrentTaskManager
    {
        private readonly ConcurrentDictionary<string, Func<Task>> _taskDict = new ConcurrentDictionary<string, Func<Task>>();

        public Task GetOrAdd(string key, Func<Task> taskFunc)
        {
            Func<Task> newTaskFunc = () =>
            {
                var task = taskFunc();
                task.ContinueWith((t) => { TryRemove(key); });
                return task;
            };

            var dictTaskFunc = _taskDict.GetOrAdd(key, newTaskFunc);
            return dictTaskFunc();
        }

        public bool TryRemove(string key)
        {
            Func<Task> taskFunc;
            return _taskDict.TryRemove(key, out taskFunc);
        }
    }
}
