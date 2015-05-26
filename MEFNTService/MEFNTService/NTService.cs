using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.ServiceProcess;

namespace MEFNTService
{
    public partial class NtService : ServiceBase
    {
        private CompositionContainer _container;
        public NtService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            StartService();
        }

        private void StartService()
        {
            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            var extensionFolder = 
                Path.GetDirectoryName(
                string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ExtensionFolder"])
                ? Assembly.GetExecutingAssembly().Location
                : ConfigurationManager.AppSettings["ExtensionFolder"]);

            if (extensionFolder != null)
            {
                catalog.Catalogs.Add(new DirectoryCatalog(extensionFolder));

                //Create the CompositionContainer with the parts in the catalog
                _container = new CompositionContainer(catalog);

                //Fill the imports of this object
                try
                {
                    _container.ComposeParts(this);
                }
                catch (CompositionException compositionException)
                {
                    Console.WriteLine(compositionException.ToString());
                }
            }
            else
            {
                throw new Exception("Extension folder is null/invalid.");
            }
        }

        protected override void OnStop()
        {
        }

        internal void DebugStartupAndStop(string[] args)
        {
            OnStart(args);
            Console.WriteLine("Service Started! Press [ENTER] to stop...");
            Console.ReadLine();
            OnStop();
        }
    }
}
