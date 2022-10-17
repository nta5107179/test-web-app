using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class Default : System.Web.UI.Page
    {
        string token = string.Empty;

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
                        configureTelemetryConfiguration: (config) => config.ConnectionString = "InstrumentationKey=1371ae0b-063d-42f9-9a7f-bc76d33f76e1;IngestionEndpoint=https://eastasia-0.in.applicationinsights.azure.com/;LiveEndpoint=https://eastasia.livediagnostics.monitor.azure.com/",
                        configureApplicationInsightsLoggerOptions: (options) => { }
                    );
                });

                IServiceProvider serviceProvider = services.BuildServiceProvider();
                ILogger<Default> logger = serviceProvider.GetRequiredService<ILogger<Default>>();

                logger.LogInformation("Information Logger is working...");
                logger.LogWarning("Warning Logger is working...");
                logger.LogError("Error Logger is working...");
                logger.LogDebug("Debug Logger is working..."); //入らなかった
                logger.LogCritical("Critical Logger is working...");
                logger.LogTrace("Trace Logger is working..."); //入らなかった
            }
            finally
            {
                // Explicitly call Flush() followed by Delay, as required in console apps.
                // This ensures that even if the application terminates, telemetry is sent to the back end.
                channel.Flush();

                //await Task.Delay(TimeSpan.FromMilliseconds(1000));
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            GetAccessToken();

            //string result = "";
            //HttpWebRequest hwr = WebRequest.CreateHttp("https://login.microsoftonline.com/" + AppSettings .Authority+ "/oauth2/authorize");
            //hwr.Method = "POST";
            //hwr.ContentType = "application/x-www-form-urlencoded";
            //byte[] data = Encoding.UTF8.GetBytes(string.Join("&", new string[] {
            //    "client_id="+AppSettings.ClientId,
            //    "response_type=code",
            //    "resource=https%3A%2F%2Fgraph.microsoft.com%2F",
            //    "redirect_uri=https%3A%2F%2Ftest-web-app2022.azurewebsites.net%2F",
            //}));
            //hwr.ContentLength = data.Length;
            //using (Stream s = hwr.GetRequestStream())
            //{
            //    s.Write(data, 0, data.Length);
            //}
            //using (WebResponse wr = hwr.GetResponse())
            //{
            //    using (StreamReader sr = new StreamReader(wr.GetResponseStream()))
            //    {
            //        result = sr.ReadToEnd();
            //    }
            //}
            //Response.Write(result);

            while (true)
            {
                if (token != string.Empty)
                {
                    Response.Write(string.Format("token:{0}", token));
                    break;
                }
                Thread.Sleep(1000);
            }

            //App Service再起動
            //string result = "";
            //string url = "https://management.azure.com/subscriptions/eb38af0b-eef2-4638-a776-4374ff5a94a6/resourceGroups/test-resource/providers/Microsoft.Web/sites/test-web-app2022/restart?api-version=2022-03-01";
            //HttpWebRequest hwr = WebRequest.CreateHttp(url);
            //hwr.Method = "POST";
            //hwr.Headers.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSIsImtpZCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSJ9.eyJhdWQiOiJodHRwczovL21hbmFnZW1lbnQuY29yZS53aW5kb3dzLm5ldCIsImlzcyI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzYzZTllNzE3LTc5ZjgtNDQ2MS1iNTllLTE0MGQyMjQ5MjBiMy8iLCJpYXQiOjE2NjU3MTUzMzAsIm5iZiI6MTY2NTcxNTMzMCwiZXhwIjoxNjY1NzE5MzcyLCJhY3IiOiIxIiwiYWlvIjoiQVdRQW0vOFRBQUFBblBqTzJKQkliNkhPLy9kOTBQWlZQL3NpZ05Mc3F1NjdDbUp1TXc3NE1saHNlVENVSkcwRzJGNTdGS0lMWm05QU5LblU5bm5XQWljNndjTHJnNklzb082SVNUc2J2ME42ejBEMWV0cG1maDE2OURaZ0dMaEpBWWxicnBLekVHL1QiLCJhbHRzZWNpZCI6IjE6bGl2ZS5jb206MDAwNjQwMDAwNjBBQjFBMSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwaWQiOiIxOGZiY2ExNi0yMjI0LTQ1ZjYtODViMC1mN2JmMmIzOWIzZjMiLCJhcHBpZGFjciI6IjAiLCJlbWFpbCI6ImhpMjAyMTAxMjZAaG90bWFpbC5jb20iLCJmYW1pbHlfbmFtZSI6IlNFSSIsImdpdmVuX25hbWUiOiJLRU4iLCJncm91cHMiOlsiNGU2NTY5MTctZDY3Ni00OTE0LWJmMGUtOWQyMzc0OGFlMTM1Il0sImlkcCI6ImxpdmUuY29tIiwiaXBhZGRyIjoiMjAyLjI0Ni4yNTIuMjI2IiwibmFtZSI6IlNFSSBLRU4iLCJvaWQiOiI0ZmRmYTY1Yy05ZDM3LTRmZTktYTZjYy1lOWU5MWI2ODExNDIiLCJwdWlkIjoiMTAwMzIwMDExMDlFRDM1MyIsInJoIjoiMC5BWEFBRi1mcFlfaDVZVVMxbmhRTklra2dzMFpJZjNrQXV0ZFB1a1Bhd2ZqMk1CTndBRDguIiwic2NwIjoidXNlcl9pbXBlcnNvbmF0aW9uIiwic3ViIjoiY3JzZzc4UWY2cGhpQVZod2dySmNvTlNIeTJJT1NvczhwVnJ0bjNoNEthRSIsInRpZCI6IjYzZTllNzE3LTc5ZjgtNDQ2MS1iNTllLTE0MGQyMjQ5MjBiMyIsInVuaXF1ZV9uYW1lIjoibGl2ZS5jb20jaGkyMDIxMDEyNkBob3RtYWlsLmNvbSIsInV0aSI6IkltN0xYaGN4akV5NFdwLU9ubG8wQUEiLCJ2ZXIiOiIxLjAiLCJ3aWRzIjpbIjYyZTkwMzk0LTY5ZjUtNDIzNy05MTkwLTAxMjE3NzE0NWUxMCIsImI3OWZiZjRkLTNlZjktNDY4OS04MTQzLTc2YjE5NGU4NTUwOSJdLCJ4bXNfdGNkdCI6MTYxMTY1NDIzNH0.N5BZO94P1Zx8aWre6fG2ZznWc4lxFXGMT9HZfRrn3pWodW8H0gOW9GxZGfMyNSTm8RnbfQSJlUz2XddTK7bLnuykmf_FFdpFJ3Jc7s1q8Yf8fEzZjoa7PbApqSgu4r7N4KakCX7ZVXN8nikigMOs5GLZoeq5u3MXO1vQE8QBjiMucpk8hEOqsm-kZnLbCRyv2WRuRKZiLp4zSB8HTV_Qeac2ACqFgcJx8Syl1BXVhdxL3D0csmp12yatetnarSU3GaEmwQlrQZS1qJWmpI8tQQY8jNikT8hUqwQAu8fZEW7yuSpJ0A3a80TY0a9e3sfGrwG9TkR_GA8_-cYaNwF-Dg");
            //hwr.ContentType = "application/json";
            //hwr.ContentLength = 0;
            //using (WebResponse wr = hwr.GetResponse())
            //{
            //    using (StreamReader sr = new StreamReader(wr.GetResponseStream()))
            //    {
            //        result = sr.ReadToEnd();
            //    }
            //}
            //Response.Write(result);
        }

        private async Task GetAccessToken()
        {
            IPublicClientApplication PublicClientApp;

            PublicClientApplicationBuilder app = PublicClientApplicationBuilder.Create(AppSettings.ClientId);
            app = app.WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient");
            app = app.WithAuthority(AzureCloudInstance.AzurePublic, AppSettings.TenantId);
            PublicClientApp = app.Build();
            //
            string[] scopes = new string[] { "User.Read" };
            string password = AppSettings.Password;
            System.Security.SecureString secpass = new System.Security.SecureString();

            foreach (char c in password) secpass.AppendChar(c);

            AcquireTokenByUsernamePasswordParameterBuilder builder = PublicClientApp.AcquireTokenByUsernamePassword(scopes, AppSettings.Account, secpass);
            AuthenticationResult authResult = await builder.ExecuteAsync();

            //textBox1.Text += "Account UserName:" + authResult.Account.Username + "\r\n";
            //textBox1.Text += "Account Environment:" + authResult.Account.Environment + "\r\n";
            //textBox1.Text += "Account HomeAccountID:" + authResult.Account.HomeAccountId + "\r\n";
            //textBox1.Text += "UniqueID:" + authResult.UniqueId + "\r\n";
            //textBox1.Text += "AccessToken:" + authResult.AccessToken + "\r\n";

            token = authResult.AccessToken;
        }
    }

    internal class AppSettings
    {
        internal static string ClientId = "b83a129c-ac81-4898-96a9-4c2cb468a827";
        internal static string ClientSecret = "9cca2a7b-316c-45eb-9031-2d19a008d472";
        internal static string TenantId = "63e9e717-79f8-4461-b59e-140d224920b3";
        internal static string[] Scopes = new string[] { "3db474b9-6a0c-4840-96ac-1fceb342124f/.default" };

        internal static string Account = "hi20210126@hotmail.com";
        internal static string Password = "1qaZ2wsx3edc";
    }
}