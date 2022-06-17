using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

namespace OtelSample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private TracerProvider tracerProvider;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var builder = Sdk.CreateTracerProviderBuilder()
                .AddAspNetInstrumentation()
                .SetSampler(new AlwaysOnSampler())
                .AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Debug);
            
            this.tracerProvider = builder.Build();
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            var act = Activity.Current; // Activity is ok here
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            // But typically null here because its not restored in the HttpModule
            // https://github.com/open-telemetry/opentelemetry-dotnet/blob/55c5dd18e9bcb8f9f728aa1a0557bd5157c88362/src/OpenTelemetry.Instrumentation.AspNet.TelemetryHttpModule/TelemetryHttpModule.cs#L88-L91
            var act = Activity.Current; 
        }
    }
}
