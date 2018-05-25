using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"], 
            ConfigurationManager.AppSettings["LuisAPIKey"], 
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Maps.SearchFuzzy")]
        public async Task MoveIntent(IDialogContext context, LuisResult result)
        {
            EntityRecommendation location;

            if (result.TryFindEntity("location", out location))
            {
                await context.PostAsync($"Search for '{location.Entity}'.");
                await InvokeIotDeviceMethod("SearchFuzzy", $"\"{location.Entity}\"");
            }
            else
            {
                await context.PostAsync("Where would you like to go?");
            }

            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Maps.ZoomIn")]
        public async Task ZoomInIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Zooming in.");
            
            try {
                await InvokeIotDeviceMethod("ZoomIn");
            }
            catch (Exception ex)
            {
                await context.PostAsync(ex.ToString());
                throw;
            }
            
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Maps.ZoomOut")]
        public async Task ZoomOutIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Zooming out.");
            await InvokeIotDeviceMethod("ZoomOut");
            await this.ShowLuisResult(context, result);
        }

        public async Task InvokeIotDeviceMethod(string methodName, string body = null, TimeSpan? timeout = null)
        {
            try
            {
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(ConfigurationManager.AppSettings["IoTHubConnectionString"]);
    
                var method = new CloudToDeviceMethod(methodName, timeout ?? TimeSpan.FromSeconds(10))
                {
                    ConnectionTimeout = TimeSpan.FromSeconds(5)
                };
    
                if (!string.IsNullOrEmpty(body))
                {
                    method.SetPayloadJson(body);
                }
    
                await serviceClient.InvokeDeviceMethodAsync("map-display", method);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                throw;                
            }
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result) 
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");
            context.Wait(MessageReceived);
        }
    }
}