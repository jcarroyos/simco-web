using System;
using System.Diagnostics;
using System.Text;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SIMCO
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        protected void Application_Error(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Error no controlado ocurrido en la aplicación: ");
            sb.AppendLine(Request.RawUrl);
            sb.Append("Query: ");
            sb.AppendLine(Request.QueryString.ToString());

            Exception ex = Server.GetLastError().GetBaseException();

           
            string err = "Error Caught in Application_Error event\n" +
                          "Error in: " + Request.Url.ToString() +
                          "\nError Message:" + ex.Message.ToString() +
                          "\nStack Trace:" + ex.StackTrace.ToString();

            EventLog.WriteEntry("SIMCO", err, EventLogEntryType.Error);
            Response.Redirect("/");
        }

    }
}
