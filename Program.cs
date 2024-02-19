using System.Text;

namespace moderator;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = OperatingSystem.IsWindows() ? Encoding.Unicode : Encoding.UTF8;
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.SetBasePath(Directory.GetCurrentDirectory());
                configuration.AddEnvironmentVariables();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel();
                webBuilder.UseUrls("http://*:1222");
                webBuilder.UseStartup<Startup>();
            });
    }
}
