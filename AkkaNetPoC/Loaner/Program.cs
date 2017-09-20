
namespace Loaner
{
    using Microsoft.AspNetCore.Hosting;
    using System.IO;

    public class Program
    {
        static void Main()
        {
           
            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }


        
    }
}