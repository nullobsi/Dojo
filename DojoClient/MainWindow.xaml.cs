using System;
using System.ServiceModel;
using System.Windows;
using DojoCommon;

namespace DojoClient
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			var factory =
				new ChannelFactory<IDojoService>(new BasicHttpBinding(),
				                                 new EndpointAddress("http://localhost:8991/dojo"));
			factory.Open();
			var channel = factory.CreateChannel();


			((IClientChannel) channel).Open();
			var r = channel.StopBreak("2dae2fd3-deff-4fc8-8f26-123025db4a4e");
			Console.WriteLine(r);
			((IClientChannel) channel).Close();
			factory.Close();
		}
	}
}