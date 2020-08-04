using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parithon.Voicemeeter.Proxy;
using Parithon.Voicemeeter.Service.Hubs;

namespace Parithon.Voicemeeter.Service
{
  public class Startup
  {
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
      Configuration = configuration;
      Environment = env;
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment {get;}

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      var builder = services.AddRazorPages();

#if DEBUG
      if (Environment.IsDevelopment()) {
        builder.AddRazorRuntimeCompilation();
      }
#endif

      services.AddSingleton<VoicemeeterRemote>();
      services.AddSingleton<VoicemeeterService>();
      services.AddHostedService(factory => factory.GetRequiredService<VoicemeeterService>());

      services.AddCors(options => {
        options.AddPolicy("CorsPolicy", builder => {
          builder.AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("https://localhost:5001")
            .AllowCredentials();
        });
      });

      services.AddSignalR();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app)
    {
      if (Environment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseCookiePolicy();
      app.UseCors("CorsPolicy");
      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapRazorPages();
        endpoints.MapHub<VoicemeeterHub>("/voicemeeter");
      });
    }
  }
}
