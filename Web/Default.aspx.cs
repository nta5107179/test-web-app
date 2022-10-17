using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
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
            //GetAccessToken();

            //token取得
            string result = "";
            string url = "https://login.microsoftonline.com/" + AppSettings.TenantId + "/oauth2/v2.0/token";
            HttpWebRequest hwr = WebRequest.CreateHttp(url);
            hwr.Method = "POST";
            hwr.ContentType = "application/x-www-form-urlencoded";
            byte[] data = Encoding.UTF8.GetBytes(string.Join("&", new string[] {
                "client_id="+AppSettings.ClientId,
                "client_secret="+AppSettings.ClientSecret,
                "scope="+AppSettings.Scopes,
                "grant_type=client_credentials"
            }));
            hwr.ContentLength = data.Length;
            using (Stream s = hwr.GetRequestStream())
            {
                s.Write(data, 0, data.Length);
            }
            using (WebResponse wr = hwr.GetResponse())
            {
                using (StreamReader sr = new StreamReader(wr.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }
            //Response.Write(result);
            JObject json = JObject.Parse(result);
            string access_token = json["access_token"].ToString();
            string token_type = json["token_type"].ToString();
            Response.Write(token_type + " " + access_token);

            //App Service再起動
            result = "";
            url = "https://management.azure.com/subscriptions/"+ AppSettings.SubscriptionId + "/resourceGroups/"+ AppSettings.ResourceGroupName + "/providers/Microsoft.Web/sites/"+ AppSettings.AppName + "/restart?api-version=2022-03-01";
            hwr = WebRequest.CreateHttp(url);
            hwr.Method = "POST";
            hwr.Headers.Add("Authorization", token_type + " " + access_token);
            hwr.ContentType = "application/json";
            hwr.ContentLength = 0;
            using (WebResponse wr = hwr.GetResponse())
            {
                using (StreamReader sr = new StreamReader(wr.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }
            Response.Write(result);
        }

        private async Task GetAccessToken()
        {

            //// Initialize MSAL library by using the following code
            //IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(AppSettings.ClientId)
            //    .WithClientSecret(AppSettings.ClientSecret)
            //    .WithAuthority(new Uri(AppSettings.TenantId))
            //    .Build();

            //// Acquire an access token
            //AuthenticationResult authertResult = null;
            //try
            //{
            //    authertResult = await app.AcquireTokenForClient(AppSettings.Scopes)
            //                    .ExecuteAsync();
            //}
            //catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            //{
            //    // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
            //    // Mitigation: change the scope to be as expected
            //    token = string.Empty;
            //    //code = "500";
            //    //message = "Scope provided is not supported";
            //    //return BadRequest(new { error = "500", error_description = "Scope provided is not supported" });
            //}
            //catch (MsalServiceException ex)
            //{
            //    // general error getting an access token
            //    token = string.Empty;
            //    //code = "500";
            //    //message = "Something went wrong getting an access token for the client API:" + ex.Message;
            //    //return BadRequest(new { error = "500", error_description = "Something went wrong getting an access token for the client API:" + ex.Message });
            //}

            //token = authertResult.AccessToken;
            //code = string.Empty;
            //message = string.Empty;
        }
    }

    internal class AppSettings
    {
        internal static string AppName = "test-web-app2022";
        internal static string ResourceGroupName = "test-resource";
        internal static string SubscriptionId = "eb38af0b-eef2-4638-a776-4374ff5a94a6";
        internal static string ClientId = "c739c898-f8ca-4428-95a8-94a4ef74bb8f";
        internal static string ClientSecret = "up48Q~QemMWRwnfwTQg-X0_9P4H1AxRZJlTQAaPG";
        internal static string TenantId = "63e9e717-79f8-4461-b59e-140d224920b3";
        internal static string Scopes = "api://c739c898-f8ca-4428-95a8-94a4ef74bb8f/.default";
        internal static string RedirectUri = "https://test-web-app2022.azurewebsites.net/";

        internal static string Account = "hi20210126@hotmail.com";
        internal static string Password = "1qaZ2wsx3edc";
    }
}