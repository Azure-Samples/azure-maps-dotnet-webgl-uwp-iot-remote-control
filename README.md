---
services: azure-maps
platforms: dotnet
author: ihvo
---

# Developing a .NET UWP application using Azure Maps, IoT hub and Bot service

This is a sample application that shows how Azure Maps, IoT hub and Bot service API can work together. We will extend the Azure Maps browser application to create a remotely controlled interactive map using Azure IoT and Azure Maps services.

## Prerequisites

Before you can run this sample, you must have the following prerequisites:

* An active Azure account. If you don't have one, you can sign up for a [free account](https://azure.microsoft.com/free/).
* Windows 10 Creators Update (Build 15063)
* Visual Studio 2017
* Universal Windows Platform tools for VS2017
* ASP.NET and web development tools for VS2017

## Getting started

1. Clone this repository using `git clone git@github.com:azure-samples/azure-maps-dotnet-webgl-uwp-iot-remote-control.git`
2. Open `MapApplication\MapApplication.sln` in VS2017.
3. Setup device connection string
    * Register a device named `map-display` and obtain device connection string by following steps from the [Device Control Quickstart](https://docs.microsoft.com/en-us/azure/iot-hub/quickstart-control-device-dotnet).
    * Open AzureIoTHub.cs and find the following section:
    ```CSharp
    //
    // Note: this connection string is specific to the device "map-display". To configure other devices,
    // see information on iothub-explorer at http://aka.ms/iothubgetstartedVSCS
    //
    const string deviceConnectionString = "<replace with your Azure IoT hub device connection string>";
    ```
    * Replace `deviceConnectionString` with connection string obtained from IoT hub in the previous step.
    
4. Setup Azure Maps web control
    * Get your Azure Maps account key. If you don't have one, please follow instructions in [demo app quickstart](https://docs.microsoft.com/en-us/azure/azure-maps/quick-demo-map-app) to create account and obtain the account key.
    * Open AzureMapDemo.html and find the following section:
    ```Javascript
    var subscriptionKey = "<replace with your Azure Maps account key>";
        var map = new atlas.Map("map", {
            "subscription-key": subscriptionKey,
            center: [-118.270293, 34.039737],
            zoom: 14
        });
    ```
    * Replace `subscriptionKey` value with your Azure Maps account key.

## Running the app

A demo app is included to show how to use the project.

To run the demo, follow these steps:

(Add steps to start up the demo)

1.
2.
3.

## More information

- [Azure Maps Documentation](https://docs.microsoft.com/en-us/azure/azure-maps/)
- [How to use the Azure Maps Map Control](https://docs.microsoft.com/en-us/azure/azure-maps/how-to-use-map-control)
- [Azure IoT Hub Documentation](https://docs.microsoft.com/azure/iot-hub/)

