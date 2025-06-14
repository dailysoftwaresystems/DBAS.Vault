using DBAS.Vault.Database.Interfaces;
using DBAS.Vault.Models;
using DBAS.Vault.Test.Properties;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DBAS.Vault.Test.Tests.Base
{
    public class BaseTest : IDisposable
    {
        private readonly APIEnvironment Environment;
        private readonly IDatabase Database;
        private readonly IServiceProvider ServiceProvider;

        public BaseTest(bool initDb = true)
        {
            (Environment, ServiceProvider) = Initialize();

            if (initDb)
            {
                Database = GetService<IDatabase>();
                Database.BeginTransaction().Wait();
            }
        }

        private (APIEnvironment, IServiceProvider) Initialize()
        {
            var assembly = Assembly.GetAssembly(typeof(Initializer.Initializer));
            string name = assembly.GetName().Name;
            string appSettingsDirectory = assembly.Location[..(assembly.Location.LastIndexOf(name) - Path.DirectorySeparatorChar.ToString().Length)];

            var builder = new ConfigurationBuilder()
                    .SetBasePath(appSettingsDirectory)
                    .AddJsonFile(Strings.BaseTest_Initialize_AppSettings)
                    .AddEnvironmentVariables();

            var config = builder.Build();
            
            var env = Initializer.Initializer.GetAPIEnvironment(config);
            env.Version = assembly.GetName().Version;

            IServiceCollection services = new ServiceCollection();
            services.Configure<APIEnvironment>(config.GetSection(nameof(Environment)));

            Initializer.Initializer.Initialize(services, env);
            return new(env, services.BuildServiceProvider());
        }

        protected virtual void CleanUp()
        {

        }

        public T GetService<T>() where T : class
        {
            return ServiceProvider.GetService<T>();
        }

        public APIEnvironment GetEnvironment()
        {
            return Environment;
        }

        public void Dispose()
        {
            try
            {
                if (Database != null)
                {
                    Database.Rollback().Wait();
                    Database.Dispose();
                }
                else
                {
                    CleanUp();
                }
            }
            catch
            {

            }

            GC.SuppressFinalize(this);
        }
    }
}
