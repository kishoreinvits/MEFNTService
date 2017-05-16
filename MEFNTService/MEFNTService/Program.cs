using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;

namespace MEFNTService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //We can use if (Environment.UserInteractive) and change application type, but I feel this is more convenient. 
            //Strictly my personal choice. You can use console class if you change application Type
            if (ConfigurationManager.AppSettings["isDebugging"].ToUpper().Equals("TRUE"))
            {
                Debugger.Break();
                //If you are passing any argumrnts to your service, Initialize here
                var args=new[]{"DummyArg1"};
                var service = new NtService();
                service.DebugStartupAndStop(args);
            }
            else
            {
                var servicesToRun = new ServiceBase[] 
                { 
                    new NtService() 
                };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
