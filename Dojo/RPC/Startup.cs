using CoreWCF;
using CoreWCF.Configuration;
using DojoCommon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Dojo.RPC
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddServiceModelServices();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseServiceModel(builder =>
			{
				builder
					.AddService<DojoService>()
					.AddServiceEndpoint<DojoService, IDojoService>(new BasicHttpBinding(), "/dojo");
			});
		}
	}
}