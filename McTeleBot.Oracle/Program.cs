using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace McTeleBot.Oracle
{
    internal class Program
    {
        public static string? DbConnStr { get; private set; }

        static Program()
        {
            var lc = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .WriteTo.Console(formatProvider: new CultureInfo("ru-RU"))
              .WriteTo.File("logs/infolog.txt", rollingInterval: RollingInterval.Day);
            Log.Logger = lc.CreateBootstrapLogger();
            // Log.Logger = lc.CreateLogger();            

        }
        static void Main(string[] args)
        {
            DbConnStr = Environment.GetEnvironmentVariable("DB_CONN") ?? "";
            if (args.Length > 0)
                DbConnStr = args[0];

            Log.Information("Start McTeleBot.Oracles !");
            Log.Information("DbConnStr = {0}", DbConnStr);

            CreateHostBuilder(args).Build().Run();

        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
           Host
               .CreateDefaultBuilder(args).UseSerilog()
               .ConfigureServices((context, collection) =>
                   collection.AddHostedService<OracleTelegramService>());
    }
}