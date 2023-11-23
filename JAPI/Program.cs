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
// setting up http client for outgoing requests
var scope = app.Services.CreateScope();

Startup.SendHandler = scope.ServiceProvider.GetService<HttpRequestHandler>();
Startup.SendHandler.SetUriValues(serviceDictionary);

/* Simulation utilities */
TelemetryHandler telemetryHandler = TelemetryHandler.Instance();
CancellationTokenSource cancellationTokenSource = new();

double executeSimulation = 3000.0; // execute the simulation every <value> milliseconds

/* Start async simulation worker */
Task simulationTask = Task.Run(() =>
{
    telemetryHandler.FlyThroughSpace("TelemetryData.json", executeSimulation, fileHandler, cancellationTokenSource.Token);
});

app.Run();

Console.ForegroundColor = ConsoleColor.Magenta;
Console.WriteLine("Stopping async simulation worker...");

cancellationTokenSource.Cancel(); // signal the simulation worker to stop

simulationTask.Wait(); // block until the simulation worker is done

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("Async simulation worker has been stopped!");
Console.ForegroundColor = ConsoleColor.White;

#region Helpers
static IHostBuilder CreateHostBuilder(string[] args) =>
     Host.CreateDefaultBuilder(args)
         .ConfigureWebHostDefaults(webBuilder =>
         {
             webBuilder.UseStartup<Startup>()
                .UseUrls("http://*:8080"); // listen on all interfaces on port 8080
         });
#endregion
