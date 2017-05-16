using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using ExtensionComponentBase;
using System.Diagnostics;

namespace MEFNTService
{
    public partial class NtService : ServiceBase
    {
        [ImportMany(typeof(IExtensionComponent))]
        private IEnumerable<IExtensionComponent> Components;

        public NtService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            StartService();
        }

        private bool StartService()
        {
            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();

            //Adds all the parts found in the same assembly as the Program class if no extension folder is defines in config
            var extensionFolder = 
                string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ExtensionFolder"])
                ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                : ConfigurationManager.AppSettings["ExtensionFolder"];

            if (!String.IsNullOrWhiteSpace(extensionFolder))
            {
                //catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
                catalog.Catalogs.Add(new DirectoryCatalog(extensionFolder));

                //Create the CompositionContainer with the parts in the catalog
                var container = new CompositionContainer(catalog);

                //Fill the imports of this object
                try
                {
                    container.ComposeParts(this); 
                    
                    foreach (IExtensionComponent component in Components)
                        component.StartAction(MEFNTServiceExceptionHandler);
                    return true;
                }
                catch (CompositionException compositionException)
                {
                    Console.WriteLine(compositionException.ToString());
                    return false;
                }
            }
            else
            {
                throw new Exception("Extension folder is null/invalid.");
            }
        }

        private void MEFNTServiceExceptionHandler(Exception obj)
        {
            Console.WriteLine(obj.Message);
            //TODO: Log or handle exceptions here
        }

        protected override void OnStop()
        {
            if (Components != null)
            {
                foreach (IExtensionComponent component in Components)
                    component.WaitForActionCompletion();
            }
        }

        internal void DebugStartupAndStop(string[] args)
        {
            Trace.WriteLine("Service Starting...");
            OnStart(args);
            Trace.WriteLine("Service Started!");
            OnStop();
            Trace.WriteLine("Service Stopped!");
        }
    }
}
