using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace WebApplication1
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var channel = new InMemoryChannel();

            try
            {
                IServiceCollection services = new ServiceCollection();
                services.Configure<TelemetryConfiguration>(config => config.TelemetryChannel = channel);
                services.AddLogging(builder =>
                {
                    // Only Application Insights is registered as a logger provider
                    builder.AddApplicationInsights(
                        configureTelemetryConfiguration: (config) => config.ConnectionString = "InstrumentationKey=61a88cdc-c36b-43cd-a5ac-763ff563bea4;IngestionEndpoint=https://japaneast-1.in.applicationinsights.azure.com/;LiveEndpoint=https://japaneast.livediagnostics.monitor.azure.com/",
                        configureApplicationInsightsLoggerOptions: (options) => { }
                    );
                });

                IServiceProvider serviceProvider = services.BuildServiceProvider();
                ILogger<Default> logger = serviceProvider.GetRequiredService<ILogger<Default>>();

                logger.LogInformation("LogInformation is working...");
                logger.LogWarning("LogWarning is working...");
                logger.LogError("LogError is working...");
                logger.LogCritical("LogCritical is working...");
                logger.LogTrace("LogTrace is working...");
                logger.LogDebug("LogDebug is working...");
            }
            finally
            {
                // Explicitly call Flush() followed by Delay, as required in console apps.
                // This ensures that even if the application terminates, telemetry is sent to the back end.
                channel.Flush();
            }
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; 
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create("https://management.azure.com/subscriptions/eb38af0b-eef2-4638-a776-4374ff5a94a6/resourceGroups/test-resource/providers/Microsoft.Web/sites/test-web-app2022/restart?api-version=2022-03-01");
            hwr.ProtocolVersion = HttpVersion.Version10;
            hwr.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            hwr.Method = "POST";
            hwr.Headers.Set("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSIsImtpZCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSJ9.eyJhdWQiOiJodHRwczovL21hbmFnZW1lbnQuY29yZS53aW5kb3dzLm5ldCIsImlzcyI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzYzZTllNzE3LTc5ZjgtNDQ2MS1iNTllLTE0MGQyMjQ5MjBiMy8iLCJpYXQiOjE2NjU2NDU0OTQsIm5iZiI6MTY2NTY0NTQ5NCwiZXhwIjoxNjY1NjUxMDM3LCJhY3IiOiIxIiwiYWlvIjoiQVdRQW0vOFRBQUFBNGdIQXBBTTJUOTlTRm5ISXdLeGlzbU12blRuZytNVS9pdkJxb0lHSnBvS0E1Z3RETDg1R2wrK05pTElZY3JxY1E4NnA1K3dERWcyaWFEVnBMY3ppbmtsUTY5SWFtOFg1ZkIyZFYxd1VRZCtQVGt2SHFTYU1abm96MlhUUkRjOWQiLCJhbHRzZWNpZCI6IjE6bGl2ZS5jb206MDAwNjQwMDAwNjBBQjFBMSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwaWQiOiIxOGZiY2ExNi0yMjI0LTQ1ZjYtODViMC1mN2JmMmIzOWIzZjMiLCJhcHBpZGFjciI6IjAiLCJlbWFpbCI6ImhpMjAyMTAxMjZAaG90bWFpbC5jb20iLCJmYW1pbHlfbmFtZSI6IlNFSSIsImdpdmVuX25hbWUiOiJLRU4iLCJncm91cHMiOlsiNGU2NTY5MTctZDY3Ni00OTE0LWJmMGUtOWQyMzc0OGFlMTM1Il0sImlkcCI6ImxpdmUuY29tIiwiaXBhZGRyIjoiMjAyLjI0Ni4yNTIuMjI3IiwibmFtZSI6IlNFSSBLRU4iLCJvaWQiOiI0ZmRmYTY1Yy05ZDM3LTRmZTktYTZjYy1lOWU5MWI2ODExNDIiLCJwdWlkIjoiMTAwMzIwMDExMDlFRDM1MyIsInJoIjoiMC5BWEFBRi1mcFlfaDVZVVMxbmhRTklra2dzMFpJZjNrQXV0ZFB1a1Bhd2ZqMk1CTndBRDguIiwic2NwIjoidXNlcl9pbXBlcnNvbmF0aW9uIiwic3ViIjoiY3JzZzc4UWY2cGhpQVZod2dySmNvTlNIeTJJT1NvczhwVnJ0bjNoNEthRSIsInRpZCI6IjYzZTllNzE3LTc5ZjgtNDQ2MS1iNTllLTE0MGQyMjQ5MjBiMyIsInVuaXF1ZV9uYW1lIjoibGl2ZS5jb20jaGkyMDIxMDEyNkBob3RtYWlsLmNvbSIsInV0aSI6IlcxUDRqdi1qbTBpd2lDNHc2VXdYQUEiLCJ2ZXIiOiIxLjAiLCJ3aWRzIjpbIjYyZTkwMzk0LTY5ZjUtNDIzNy05MTkwLTAxMjE3NzE0NWUxMCIsImI3OWZiZjRkLTNlZjktNDY4OS04MTQzLTc2YjE5NGU4NTUwOSJdLCJ4bXNfdGNkdCI6MTYxMTY1NDIzNH0.pCwdrhLyHxhm7Ru038dqvwyuXsnN8YOcS9YWvabwEk49yyqmDfp4RrF6rMHg-sKJB9C3-w6hWW-L5TRKgzkjnxJTNNlPo9ZXeS8oVQhIZM1fuzSErqjzagftcJRVxNQNtPEJ4t_frh5HqABLiSJmHggJOktpQ5P9lxNrzYIEpPMaNsaELibSz5uowQQwJCj27wxYbOtHyZW1PuGFWu69fc4i8lj2hELdjQaTJZfk6tj0RnM1l4ExXyc2nr7g23tQn35V9XUlSmW45EmE83QlzLkmWz5mkrmE1VGrIlzcN4XjKzGxBfA-bfKi_saXDSJ8MSGRqfUXEjYad-q2F0boPw");
            hwr.ContentType = "application/json";
            hwr.ContentLength = 0;
            string str = "";
            using (WebResponse wr = hwr.GetResponse())
            {
                using (StreamReader sr = new StreamReader(wr.GetResponseStream()))
                {
                    str = sr.ReadToEnd();
                }
                
            }
            Response.Write(str);
            Response.End();
        }
    }
}