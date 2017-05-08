using Compliance.Common.GenericRepo.Implementation;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.ScoreCards.Domain;
using Compliance.ScoreCards.Domain.Services;
using Compliance.ScoreCards.Implementation.Ef;
using Compliance.ScoreCards.Implementation.Ef.Data;
using Compliance.ScoreCards.Implementation.Ef.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Compliance.ScoreCards.Host.AspNetCore
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc();
			services.AddTransient<IMakeDbContext<ScoreCardsContext>, ScoreCardContextFromConfig>();
			services.AddTransient<IGenericRepo<ScoreCard, ScoreCardsContext>, GenericRepo<ScoreCard, ScoreCardsContext>>();
			services.AddTransient<IGenericRepo<ScoreCardResult, ScoreCardsContext>, GenericRepo<ScoreCardResult, ScoreCardsContext>>();
			services.AddTransient<IGenericRepo<ScoreCardReview, ScoreCardsContext>, GenericRepo<ScoreCardReview, ScoreCardsContext>>();
			services.AddTransient<IScoreCardService, ScoreCardService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseMvc();
		}
	}
}
