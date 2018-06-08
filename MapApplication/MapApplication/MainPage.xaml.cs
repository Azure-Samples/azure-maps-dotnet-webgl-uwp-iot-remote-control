using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MapApplication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void webView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            await AzureIoTHub.RegisterDirectMethodAsync(
                "SearchFuzzy",
                 async delegate (string query)
                 {
                     System.Diagnostics.Debug.WriteLine($"Request for 'SearchFuzzy' is received.");
                     await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                         async () => await webView.InvokeScriptAsync("eval", new[] { $"document.getElementById('search-input').value = '{query}'; shouldChangeCamera = true; search(searchResultsHandler);" })
                    );
                 });

            await AzureIoTHub.RegisterDirectMethodAsync(
                "ZoomIn",
                 async delegate (string query)
                 {
                     System.Diagnostics.Debug.WriteLine($"Request for 'ZoomIn' is received.");
                     await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                         async () => await webView.InvokeScriptAsync("eval", new[] { $"plusZoomElement.click();" })
                    );
                 });

            await AzureIoTHub.RegisterDirectMethodAsync(
                "ZoomOut",
                 async delegate (string query)
                 {
                     System.Diagnostics.Debug.WriteLine($"Request for 'ZoomOut' is received.");
                     await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                         async () => await webView.InvokeScriptAsync("eval", new[] { $"minusZoomElement.click();" })
                    );
                 });
        }
    }
}
