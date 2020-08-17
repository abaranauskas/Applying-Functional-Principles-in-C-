using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using CustomerManagement.Api.Utils;
using CustomerManagement.Logic.Utils;

namespace CustomerManagement.Api
{
    public class WebApiApplication : HttpApplication
    {
        private const string ConnectionString = "Server=.;Database=CustomerManagement;Trusted_Connection=true;";

        protected void Application_Start()
        {
            InitContainer();
            InitWebApi();

            Initer.Init(ConnectionString);
        }

        private void InitContainer()
        {
            DIContainer.Init();
            GlobalConfiguration.Configuration.DependencyResolver = DIContainer.GetDependencyResolver();
        }

        private void InitWebApi()
        {
            HttpConfiguration config = GlobalConfiguration.Configuration;

            config.MapHttpAttributeRoutes();
            config.Formatters.Add(new BrowserJsonFormatter());
            config.Services.Add(typeof(IExceptionLogger), new Utils.ExceptionLogger());
            config.Services.Replace(typeof(IExceptionHandler), new GenericTexExceptionHandler());

            config.EnsureInitialized();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

        }
    }
}
