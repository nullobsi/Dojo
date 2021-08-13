using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace Dojo.UI
{
	public partial class MainWindow
	{
		// code ninjas location
		private string _location;

		// keep pinging for data
		private Timer _timer;

		// removed cards
		private readonly ObservableCollection<ScanInData> hidden;

		public MainWindow()
		{
			// load all images (static constructor)
			ScanInData.Belts["white"].GetType();
			// change WebBrowser control to use latest version of IE
			var regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey("Microsoft")
			        .CreateSubKey("Internet Explorer")
			        .CreateSubKey("Main")
			        .CreateSubKey("FeatureControl")
			        .CreateSubKey("FEATURE_BROWSER_EMULATION");
			regKey.SetValue(AppDomain.CurrentDomain.FriendlyName, 11001, RegistryValueKind.DWord);
			regKey.SetValue(AppDomain.CurrentDomain.FriendlyName + ".exe", 11001, RegistryValueKind.DWord);
			InitializeComponent();
			// log in to Dojo
			Browser.Navigated += Browser_Navigated;
			Browser.Navigate("https://dojo.code.ninja/stafflogin");
			Browser.LoadCompleted += Browser_LoadCompleted;
			BrowserProxy.OnData += OnScanIn;
			hidden = new ObservableCollection<ScanInData>();
			hideCard += card => hidden.Add(card);
			RemovedLv.ItemsSource = hidden;
		}

		public static void HideCard(ScanInData c)
		{
			hideCard?.Invoke(c);
		}

		private static event Action<ScanInData> hideCard;

		private void OnScanIn(ScanInData n)
		{
			MainWrapPanel.Children.Add(new NinjaCard(n));
		}

		private void HideScriptErrors(WebBrowser wb)
		{
			// hack to prevent browser from popping up errors
			dynamic val = Browser.GetType().InvokeMember("ActiveXInstance",
			                                             BindingFlags.Instance | BindingFlags.NonPublic |
			                                             BindingFlags.GetProperty, null, Browser, new object[0]);
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
				AutoReset = true
			};
			_timer.Elapsed += FetchDataPeriodically;
			_timer.Start();
		}

		private void FetchDataPeriodically(object sender, ElapsedEventArgs e)
		{
			Dispatcher.BeginInvoke((Action) delegate
			{
				var eval =
					$"try{{window.$.get('https://dojo.code.ninja/api/employee/{_location}/scanins/420', '', function(data){{window.external.JSGotData(JSON.stringify(data.scanIns));}});}}catch(e){{window.external.JSGotError(e);}}";
				Browser.InvokeScript(
					"eval",
					eval);
			});

			foreach (var scanInData in hidden)
			{
				if (scanInData.MinutesLeft > -5) continue;
				scanInData.Dispose();
				hidden.Remove(scanInData);
			}
		}

		private void ExitBtnClicked(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void MinBtnClicked(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void RemovedLv_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var item = RemovedLv.SelectedItem as ScanInData;
			hidden.Remove(item);
			OnScanIn(item);
		}
	}
}
