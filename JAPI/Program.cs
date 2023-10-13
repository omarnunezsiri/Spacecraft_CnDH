using JAPI;

#region JAPI
CreateHostBuilder(args).Build().Run();
#endregion


#region Helpers

static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

#endregion