// The Spacecraft C&DH Team licenses this file to you under the MIT license.

using JAPI;
using JAPI.Handlers;

#region Init/Setup
FileHandler fileHandler = FileHandler.GetFileHandler(); // retrieve file handler singleton instance
Dictionary<int, string> serviceDictionary = new Dictionary<int, string>();
try
{
    serviceDictionary = fileHandler.ReadIpConfigFile("ips.cfg"); // read the service key-value pairs
}
catch (Exception e)
{
    /* Display error message */
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Exception caught: {e.Message}\nExiting...");
    Console.ForegroundColor = ConsoleColor.White;

    /* Exit app (failure) */
    Environment.Exit(1);
}
#endregion

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Service IpConfigFile reading completed without any errors.");
Console.ForegroundColor = ConsoleColor.White;

var app = CreateHostBuilder(args).Build();
using (var scope = app.Services.CreateScope())
{
    Startup.SendHandler = scope.ServiceProvider.GetService<HttpRequestHandler>();
    Startup.SendHandler.SetUriValues(serviceDictionary);
}
app.Run();

#region Helpers
static IHostBuilder CreateHostBuilder(string[] args) =>
     Host.CreateDefaultBuilder(args)
         .ConfigureWebHostDefaults(webBuilder =>
         {
             webBuilder.UseStartup<Startup>()
                .UseUrls("http://*:8080"); // listen on all interfaces on port 8080
         });
#endregion
