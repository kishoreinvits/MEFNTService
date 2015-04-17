using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Reflection;
using System.ServiceProcess;

namespace MEFNTService
{
    public partial class NTService : ServiceBase
    {
        private CompositionContainer _container;
        public NTService()
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
            string ExtensionFolder = 
                string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ExtensionFolder"]) 
                ? Assembly.GetExecutingAssembly().Location 
                : ConfigurationManager.AppSettings["ExtensionFolder"];
            
            catalog.Catalogs.Add(new DirectoryCatalog(ExtensionFolder));

            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        protected override void OnStop()
        {
        }

        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }
    }
}
