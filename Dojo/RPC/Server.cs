using System;
using Makaretu.Dns;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Dojo.RPC
{
	public class Server
	{
		public Server()
		{
			Console.WriteLine("Starting Dojo server...");
			var host = CreateWebHostBuilder(new string[] { }).Build();
			host.RunAsync();
			var mdns = new MulticastService();

			foreach (var a in MulticastService.GetIPAddresses()) Console.WriteLine($"IP Address {a}");

			var sd = new ServiceDiscovery(mdns);
			sd.Advertise(new ServiceProfile("dojo", "_dojo._tcp", 8991));

			mdns.Start();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
			              .UseKestrel(options => { options.ListenLocalhost(8991); })
			              .UseStartup<Startup>();
		}
	}
}