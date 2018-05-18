using Autofac;
using Autofac.Extensions.DependencyInjection;
using Backend.Extensions;
using Backend.Infrastucture;
using Backend.Middleware;
using Backend.Modules;
using Core.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NBsoft.Logs;
using NBsoft.Logs.Interfaces;
using NBsoft.Logs.Sql;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Threading.Tasks;

namespace Backend
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; set; }

        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .Build();

            Environment = env;
        }

        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var loggerFactory = new LoggerFactory()
                .AddConsole(LogLevel.Error)
                .AddDebug(LogLevel.Error);

            services.AddSingleton(loggerFactory);
            services.AddLogging();
            services.AddSingleton(Configuration);

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });
            services.AddAuthentication(KeyAuthOptions.AuthenticationScheme)
                .AddScheme<KeyAuthOptions, KeyAuthHandler>(KeyAuthOptions.AuthenticationScheme, "", options => { });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Version = "v1", Title = "BoTest" });
                options.DescribeAllEnumsAsStrings();
                options.OperationFilter<ApiKeyHeaderOperationFilter>();
            });

            var builder = new ContainerBuilder();

            var settings = Configuration.Get<BackendSetting>();

            SetupLoggers(services, settings);

            RegisterModules(builder, settings);

            builder.Populate(services);

            ApplicationContainer = builder.Build();
            
            ApplicationContainer.InitUsersRepository();
            
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "BoTest v1");
            });
            app.UseStaticFiles();
        }

        private static void SetupLoggers(IServiceCollection services, BackendSetting settings)
        {
            var loggerAggregate = new LoggerAggregate();
            loggerAggregate.AddLogger(new ConsoleLogger());
            loggerAggregate.AddLogger(new SqlServerLogger(settings.Db.LogConnString,"BoTestLogs"));

            services.AddSingleton<NBsoft.Logs.Interfaces.ILogger>(loggerAggregate);
            Program.Log = loggerAggregate;

            Task.Run(async () => await Program.Log.WriteInfoAsync("Backend", "Startup", null, "Logger Started"));
        }

        private void RegisterModules(ContainerBuilder builder, BackendSetting settings)
        {
            builder.RegisterModule(new SettingsModule(settings));
            builder.RegisterModule(new RepositoriesModule(settings, Program.Log));
            builder.RegisterModule(new ServicesModule(settings, Program.Log));
            

        }
    }
}
