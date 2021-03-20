using System;
using System.Windows;

namespace Dojo
{
	/// <summary>
	///     Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			Console.WriteLine("Started!");
			base.OnStartup(e);
		}
	}
}