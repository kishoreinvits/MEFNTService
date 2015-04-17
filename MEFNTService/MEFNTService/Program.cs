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
            //We can use if (Environment.UserInteractive) and change application type, but I feel this is more convenient. Strictly my personal choice
            if (ConfigurationManager.AppSettings["isDebugging"].ToUpper().Equals("TRUE"))
            {
                Debugger.Break();
                //If you are passing any argumrnts to your service, Initialize here
                string[] args=new string[]{"DummyArg1"};
                NTService service1 = new NTService();
                service1.TestStartupAndStop(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new NTService() 
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
