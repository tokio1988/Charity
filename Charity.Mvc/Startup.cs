﻿using Charity.Mvc.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Charity.Mvc.Services;
using Charity.Mvc.Services.Interfaces;
using System.IO;

namespace Charity.Mvc
{
	public class Startup
    {

        protected IConfigurationRoot Configuration;

        public Startup()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
		{
            services.AddDbContext<CharityContext>(builder =>
            {
                var connectionString = Configuration["ConnectionString:DataSource"];
                builder.UseSqlServer(connectionString);
            });

            

            services.AddScoped<IInstitutionService, InstitutionService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IDonationService, DonationService>();
            services.AddScoped<ICategoriesForDonations, CategoriesForDonationsService>();

            services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

            app.UseMvcWithDefaultRoute();
            app.UseStaticFiles();
            app.UseMvc();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Donation}/{action=Donate}");
            });
        }
	}
}
