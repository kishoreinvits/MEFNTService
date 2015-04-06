using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

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
    }
}
