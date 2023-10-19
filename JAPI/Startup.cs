// The Spacecraft C&DH Team licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;

namespace JAPI;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddRouting();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure the HTTP request pipeline.
        if (app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        #region Endpoints
        app.UseEndpoints(endpoints =>
        {
            // Telemetry Request
            endpoints.MapGet("/telemetry", ([FromQuery(Name = "ID")] int source, HttpContext ctx) =>
            {
                /* Configure the response */
                ctx.Response.StatusCode = StatusCodes.Status501NotImplemented;
            })
            .WithName("telemetry")
            .WithOpenApi();

            // Point command
            endpoints.MapPut("/route", ([FromQuery(Name = "ID")] int source, HttpContext ctx) =>
            {
                /* Configure the response */
                ctx.Response.StatusCode = StatusCodes.Status501NotImplemented;
            })
            .WithName("route")
            .WithOpenApi();

            // Download image route
            endpoints.MapPost("/downloadImage", (HttpContext ctx) =>
            {
                /* Configure the response */
                ctx.Response.StatusCode = StatusCodes.Status501NotImplemented;
            })
            .WithName("Download Image")
            .WithOpenApi();

            // Payload On Off
            endpoints.MapPut("/payloadState", ([FromQuery(Name = "ID")] int source, [FromQuery(Name = "state")] bool state, HttpContext ctx) =>
            {
                /* Configure the response */
                ctx.Response.StatusCode = StatusCodes.Status501NotImplemented;
            })
            .WithName("Payload Power")
            .WithOpenApi();
        });
        #endregion
    }
}
