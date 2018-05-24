using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

class AzureIoTHub
{
    private static void CreateClient()
    {
        if (deviceClient == null)
        {
            // create Azure IoT Hub client from embedded connection string
            deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
        }
    }

    static DeviceClient deviceClient = null;

    //
    // Note: this connection string is specific to the device "map-display". To configure other devices,
    // see information on iothub-explorer at http://aka.ms/iothubgetstartedVSCS
    //
    const string deviceConnectionString = "<replace with your Azure IoT hub device connection string>";

    // Refer to http://aka.ms/azure-iot-hub-vs-cs-2017-wiki for more information on Connected Service for Azure IoT Hub

    public static async Task SendDeviceToCloudMessageAsync()
    {
        CreateClient();
#if WINDOWS_UWP
        var str = "{\"deviceId\":\"map-display\",\"messageId\":1,\"text\":\"Hello, Cloud from a UWP C# app!\"}";
#else
        var str = "{\"deviceId\":\"map-display\",\"messageId\":1,\"text\":\"Hello, Cloud from a C# app!\"}";
#endif
        var message = new Message(Encoding.ASCII.GetBytes(str));

        await deviceClient.SendEventAsync(message);
    }

    public static async Task<string> ReceiveCloudToDeviceMessageAsync()
    {
        CreateClient();

        while (true)
        {
            var receivedMessage = await deviceClient.ReceiveAsync();

            if (receivedMessage != null)
            {
                var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                await deviceClient.CompleteAsync(receivedMessage);
                return messageData;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }

    public static async Task GetDeviceTwinAsync()
    {
        CreateClient();

        Console.WriteLine("Getting device twin");
        Twin twin = await deviceClient.GetTwinAsync();
        Console.WriteLine(twin.ToJson());
    }

    private static Task OnDesiredPropertiesUpdated(TwinCollection desiredProperties, object userContext)
    {
        Console.WriteLine("Desired properties were updated");
        Console.WriteLine(desiredProperties.ToJson());
        return Task.CompletedTask;
    }

    public static async Task RegisterTwinUpdateAsync()
    {
        CreateClient();

        Console.WriteLine("Registering Device Twin update callback");
        await deviceClient.SetDesiredPropertyUpdateCallback(OnDesiredPropertiesUpdated, null);
    }

    public static async Task UpdateDeviceTwin()
    {
        CreateClient();

        TwinCollection tc = new TwinCollection();
        tc["SampleProperty1"] = "test value";

        Console.WriteLine("Updating Device Twin reported properties");
        await deviceClient.UpdateReportedPropertiesAsync(tc);
    }

    public static async Task RegisterDirectMethodAsync(string methodName, Func<string, Task> action)
    {
        CreateClient();

        System.Diagnostics.Debug.WriteLine($"Registering a callback for {methodName}");
        await deviceClient.SetMethodHandlerAsync(methodName,
            async delegate (MethodRequest methodRequest, object userContext)
            {
                System.Diagnostics.Debug.WriteLine($"{methodName} has been called");
                try
                {
                    await action(methodRequest.DataAsJson);
                    return new MethodResponse(200);
                }
                catch (Exception ex)
                {
                    return new MethodResponse(Encoding.UTF8.GetBytes(ex.ToString()), 500);
                }
            }, null);
    }
}
