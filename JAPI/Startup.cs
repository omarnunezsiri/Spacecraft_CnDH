// The Spacecraft C&DH Team licenses this file to you under the MIT license.

using System.Text;
using System.Text.Json;
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
                Telemetry? getTelemetryData = new Telemetry();
                FileHandler fileHandler = FileHandler.GetFileHandler();
                getTelemetryData = fileHandler.ReadTelemetryData("TelemetryData.json");

                if (getTelemetryData == null)
                {
                    ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    return;
                }
                else
                {
                    ctx.Response.StatusCode = StatusCodes.Status200OK;
                    ctx.Response.CompleteAsync();
                }

                var sendData = JsonSerializer.Serialize(getTelemetryData);
                var requestContent = new StringContent(sendData, Encoding.UTF8, "application/json");

                SendHandler.SendPackagedData(requestContent, source);
            })
            .WithName("telemetry")
            .WithOpenApi();

            // Point command
            endpoints.MapPut("/point", async ([FromQuery(Name = "ID")] int source, HttpContext ctx) =>
            {
                //Check if chargin
                TelemetryHandler handler = TelemetryHandler.Instance();
                if (handler.GetTelemetry().status.chargeStatus == true)
                {
                    ctx.Response.StatusCode = StatusCodes.Status405MethodNotAllowed; //Charging State => Turn off for requests
                    return;
                }

                using (StreamReader reader = new StreamReader(ctx.Request.Body, Encoding.UTF8))
                {
                    var requestBody = reader.ReadToEndAsync();

                    // Create an HttpContent from the request body
                    var requestContent = new StringContent(requestBody.Result, Encoding.UTF8, "application/json");
                    string requestConverted = requestContent.ReadAsStringAsync().Result;
                    Telemetry payload = JsonSerializer.Deserialize<Telemetry>(requestConverted);

                    //Check if the required parameters are present
                    if (payload.coordinate == null || payload.rotation == null)
                    {
                        ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                        return;
                    }

                    //Retrieve the direction from the payload
                    float xCoord = payload.coordinate.x;
                    float yCoord = payload.coordinate.y;
                    float zCoord = payload.coordinate.z;

                    float pitch = payload.rotation.p;
                    float yaw = payload.rotation.y;
                    float roll = payload.rotation.r;

                    if (!handler.GetTelemetry().UpdateShipDirection(xCoord, yCoord, zCoord, pitch, yaw, roll))
                    {
                        ctx.Response.StatusCode = StatusCodes.Status501NotImplemented; //Not right status code (Not Tested)
                        return;
                    }
                    else
                    {
                        //Everything is good - COPIUM
                        ctx.Response.StatusCode = StatusCodes.Status200OK;
                        return;
                    }
                }

            })
            .WithName("point")
            .WithOpenApi();

            // Download image route
            endpoints.MapPost("/downloadImage", async (HttpContext ctx) =>
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
                    await SendHandler.SendRawData(requestContent).ConfigureAwait(true);
                }

            })
            .WithName("Download Image")
            .WithOpenApi();

            // Payload On Off
            endpoints.MapPut("/payloadState", ([FromQuery(Name = "ID")] int source, [FromQuery(Name = "state")] bool state, HttpContext ctx) =>
            {
                Dictionary<int, string> validUris = SendHandler.GetUriValues();
                // check that id is in dictionary
                if (!validUris.ContainsKey(source))
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                //Check payload status
                TelemetryHandler handler = TelemetryHandler.Instance();
                if (handler.GetTelemetry().status.payloadPower == state)
                {
                    ctx.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    return;
                }
                /* Configure the response */
                ctx.Response.StatusCode = StatusCodes.Status200OK;
                ctx.Response.CompleteAsync();
                SendHandler.TogglePayload(state);

            })
            .WithName("Payload Power")
            .WithOpenApi();
        });
        #endregion
    }
}
