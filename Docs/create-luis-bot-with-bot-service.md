In this guid we will build a sample Bot service that will remotely control the MapApplication using text input. This service can also be enabled as Cortana skill to support voice commands.

## Create a Language Understanding bot with Bot Service

1. In the Azure portal, select Create new resource in the menu blade and click See all.
2. In the search box, search for Web App Bot.
3. In the Bot Service blade, provide the required information, and click Create. This creates and deploys the bot service and LUIS app to Azure.
    * Set App name to your bot’s name. The name is used as the subdomain when your bot is deployed to the cloud (for example, mynotesbot.azurewebsites.net). This name is also used as the name of the LUIS app associated with your bot. Copy it to use later, to find the LUIS app associated with the bot.
    * Select the subscription, resource group, App service plan, and location.
    * Select the Language understanding (C#) template for the Bot template field.
    ![Web App Bot Dialog](./Media/create-luis-bot-with-bot-service/Image1.png)
    * Check the box to confirm to the terms of service.
4. Confirm that the bot service has been deployed.
    * Click Notifications (the bell icon that is located along the top edge of the Azure portal). The notification will change from **Deployment started** to **Deployment succeeded**.
    * After the notification changes to **Deployment succeeded**, click **Go to resource** on that notification.

## Try the bot

Confirm that the bot has been deployed by checking the **Notifications**. The notifications will change from **Deployment in progress...** to **Deployment succeeded**. Click **Go to resource** button to open the bot's resources blade.

Once the bot is registered, click Test in Web Chat to open the Web Chat pane. Type "hello" in Web Chat. The bot responds by saying "1: You said hello". This confirms that the bot has received your message and passed it to a default LUIS app that it created. This default LUIS app detected a Greeting intent.

## Modify the LUIS app
Log in to [laui.ai](https://www.luis.ai) using the same account you use to log in to Azure. Click on **My apps**. In the list of apps, find the app that begins with the name specified in **App name** in the **Bot Service** blade when you created the Bot Service.

The LUIS app starts with 4 intents: Cancel, Greeting, Help, and None.

The following steps add the Maps.SearchFuzzy, Maps.ZoomIn, and Maps.ZoomOut intents:
1. Click the **Create new intent** button, set the name as **Maps.SearchFuzzy** and click **Done**.
    * Add the following utterances _Go to Seattle_, _Show me Seattle_, _Search for Seattle_.
	* Create **Location** entity. To do that click the word _Seattle_ in the first utterance, wait for the context menu to show up and type _location_. In the popup dialog select **Simple** entity type.
    ![Location entity dialog](./Media/create-luis-bot-with-bot-service/Image2.png)
    * Repeat the sequence for each occurrence of _Seattle_ in other utterances. However, this time choose existing **location** entity at the bottom of context menu.

    ![Set location entity](./Media/create-luis-bot-with-bot-service/Image3.png)

2. Add intents with names **Maps.ZoomIn** and **Maps.ZoomOut** each having one utterance - _Zoom in_ and _Zoom out_ respectively.
3. In the **Intents** page, click on each of the following intent names and then click the **Delete Intent** button.
	* Cancel
	* Greeting
	* Help
		
	The only intents that should remain in the LUIS app are the following:
	* Maps.SearchFuzzy
	* Maps.ZoomIn
	* Maps.ZoomOut
	* None

    ![List of intents](./Media/create-luis-bot-with-bot-service/Image4.png)

4. Click the **Train** button in the upper right to train your app.

5. Click **PUBLISH** in the top navigation bar to open the **Publish** page. Click the **Publish to production** slot button. After successful publish, copy the URL displayed in the **Endpoint** column the **Publish App** page, in the row that starts with the Resource Name Starter_Key. 

## Modify the bot code

1. Open bot service in Azure portal. Click **Build** and then click **Open online code editor**.
    ![List of intents](./Media/create-luis-bot-with-bot-service/Image5.png)
2. In the code editor, open BasicLuisDialog.cs. By default it contains sample intent handlers.
3. Add the following using statement in BasicLuisDialog.cs.

    ```using Microsoft.Azure.Devices;```

4. Add the following code within the BasicLuisDialog class, after the constructor definition.

```CSharp
    [LuisIntent("Maps.SearchFuzzy")]
    public async Task MoveIntent(IDialogContext context, LuisResult result)
    {
        EntityRecommendation location;
 
        if (result.TryFindEntity("location", out location))
        {
            await context.PostAsync($"Search for '{location.Entity}'.");
            await InvokeIotDeviceMethod("SearchFuzzy", $"\"{location.Entity}\"");
            }
            else
            {
                await context.PostAsync("Where would you like to go?");
            }
 
            await this.ShowLuisResult(context, result);
        }
 
        [LuisIntent("Maps.ZoomIn")]
        public async Task ZoomInIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Zooming in.");
            await InvokeIotDeviceMethod("ZoomIn");
            await this.ShowLuisResult(context, result);
        }
 
        [LuisIntent("Maps.ZoomOut")]
        public async Task ZoomOutIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Zooming out.");
            await InvokeIotDeviceMethod("ZoomOut");
            await this.ShowLuisResult(context, result);
        }
 
        public async Task InvokeIotDeviceMethod(string methodName, string body = null, TimeSpan? timeout = null)
        {
            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(ConfigurationManager.AppSettings["IoTHubConnectionString"]);
 
            var method = new CloudToDeviceMethod(methodName, timeout ?? TimeSpan.FromSeconds(10))
            {
                ConnectionTimeout = TimeSpan.FromSeconds(5)
            };
 
            if (!string.IsNullOrEmpty(body))
            {
                method.SetPayloadJson(body);
            }
 
            await serviceClient.InvokeDeviceMethodAsync("map-display", method); // set device name here to the one you provisioned in IoT hub
        }

```

## Reference
[Connect an existing bot to Cortana](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-cortana?view=azure-bot-service-3.0)
