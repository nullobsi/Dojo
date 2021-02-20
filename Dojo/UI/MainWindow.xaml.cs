using System;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace Dojo
{
	
	public partial class MainWindow
	{
		// keep pinging for data
		private Timer _timer;
		// code ninjas location
		private string _location;

		public MainWindow()
		{
			// change WebBrowser control to use latest version of IE
			Registry.CurrentUser.OpenSubKey("SOFTWARE", writable: true).CreateSubKey("Microsoft").CreateSubKey("Internet Explorer")
				.CreateSubKey("Main")
				.CreateSubKey("FeatureControl")
				.CreateSubKey("FEATURE_BROWSER_EMULATION")
				.SetValue(AppDomain.CurrentDomain.FriendlyName, 11001, RegistryValueKind.DWord);
			InitializeComponent();
			// log in to Dojo
			Browser.Navigated += Browser_Navigated;
			Browser.Navigate("https://dojo.code.ninja/stafflogin");
			Browser.LoadCompleted += Browser_LoadCompleted;
			BrowserProxy.OnData += OnScanIn;
		}

		private void OnScanIn(ScanInData n)
		{
			MainWrapPanel.Children.Add(new NinjaCard(n));
		}

		private void HideScriptErrors(WebBrowser wb)
		{
			// hack to prevent browser from popping up errors
			dynamic val = Browser.GetType().InvokeMember("ActiveXInstance", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty, null, Browser, new object[0]);
			val.Silent = true;
		}

		private void Browser_Navigated(object sender, NavigationEventArgs e)
		{
			HideScriptErrors(Browser);
		}

		private void Browser_LoadCompleted(object sender, NavigationEventArgs e)
		{
			if (!e.Uri.AbsolutePath.StartsWith("/employees")) return;
			// has start and end slash
			_location = e.Uri.AbsolutePath.Replace("/employees/", "").Replace("/", "");
			Console.WriteLine("Logged in to " + _location);
			// add browser proxy and hide browser, maximise
			Browser.ObjectForScripting = new BrowserProxy();
			Browser.Visibility = Visibility.Collapsed;
			WindowStyle = WindowStyle.None;
			WindowState = WindowState.Maximized;
			// initialize timer
			_timer = new Timer
			{
				Interval = 5000.0,
				AutoReset = true,
			};
			_timer.Elapsed += FetchDataPeriodically;
			_timer.Start();
			// fetch initial data
			Browser.InvokeScript("eval", $"$.get(\"https://dojo.code.ninja/api/employee/{_location}/scanins/420\",\"\",function(data){{ window.external.JSGotData(JSON.stringify(data.scanIns)) }})");
		}

		private void FetchDataPeriodically(object sender, ElapsedEventArgs e)
		{
			Dispatcher.BeginInvoke((Action)delegate
			{
				Browser.InvokeScript("eval", $"$.get(\"https://dojo.code.ninja/api/employee/{_location}/scanins/420\",\"\",function(data){{ window.external.JSGotData(JSON.stringify(data.scanIns)) }})");
			});
		}
	}
}
