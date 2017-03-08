using System.IO;
using Microsoft.AspNetCore.Hosting;
using Contextually;
using System.Collections.Specialized;

namespace NTier
{

    public class Program
    {
        public static void Main(string[] args)
        {
            using(Relevant.Info(new NameValueCollection 
            {
                ["ExecutingDirectory"] = Directory.GetCurrentDirectory()
            }))
            {

                var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    .Build();

                host.Run();
            }
        }
    }
}
