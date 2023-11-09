// The Spacecraft C&DH Team licenses this file to you under the MIT license.

using System.Text;
using JAPI.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace JAPI;

public class Startup
{
    public static HttpRequestHandler SendHandler { get; set; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddRouting();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHttpClient();
        services.AddScoped<HttpRequestHandler>();
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
                ctx.Response.StatusCode = StatusCodes.Status204NoContent;

                /* Send Response*/
                ctx.Response.CompleteAsync();

                /* Required Action */
                // Read the content from the request body
                using (StreamReader reader = new StreamReader(ctx.Request.Body, Encoding.UTF8))
                {
                    var requestBody = reader.ReadToEndAsync();

                    // Create an HttpContent from the request body
                    var requestContent = new StringContent(requestBody.Result, Encoding.UTF8, "application/json");
                    SendHandler.SendRawData(requestContent);
                }

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
