using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ExtensionComponentBase;

namespace ExtensionComponents
{
    [Export(typeof(IExtensionComponent))]
    public class LogMonitoringComponent : IExtensionComponent
    {
        private readonly List<Task> _tasks = new List<Task>();
        public void StartAction(System.Action<System.Exception> exceptionCallBack)
        {
            var task = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Debug.Write("Running LogMonitoringComponent...");
                    Thread.Sleep(1000);
                }
                Debug.Write("LogMonitoringComponent Completed...");
                //ToDo:Monitor Logs for any errors and report if any are found.
            });
            _tasks.Add(task);
        }

        public void WaitForActionCompletion()
        {
            Task.WaitAll(_tasks.ToArray());
        }
    }
}
