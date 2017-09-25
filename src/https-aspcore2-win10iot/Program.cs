using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Net;

namespace https_aspcore2_win10iot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Listen(
                        IPAddress.Any,
                        5000,
                        listenOptions =>
                        {
                            listenOptions.UseHttps("testCertificate.pfx", "p@ssw0rd");
                        }
                    );
                })
                .Build();
        }
    }
}
