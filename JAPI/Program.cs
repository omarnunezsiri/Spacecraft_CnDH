// The Spacecraft C&DH Team licenses this file to you under the MIT license.

using JAPI.Handlers;
using Microsoft.AspNetCore.Mvc;

#region Setup
//This is where any setup code will go.
FileHandler fileHandler = FileHandler.GetFileHandler();
Dictionary<int, string> serviceDictionary = new Dictionary<int, string>();
try
{
    serviceDictionary = fileHandler.ReadIpConfigFile("ips.cfg");
}
catch (Exception) { Environment.Exit(1); }
#endregion

#region Config
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endregion

#region Endpoints
# if DEBUG
// test route
app.MapGet("/", (HttpContext ctx) =>
{
    /* Configure the response */
    ctx.Response.StatusCode = StatusCodes.Status200OK;

    /* Payload */
    TelemetryHandler telemetryHandler = new TelemetryHandler();

    object telemetry = telemetryHandler.GetTelemetry();

    return telemetry;
})
.WithName("Root")
.WithOpenApi();
#endif

// Telemetry Request
app.MapGet("/telemetry", ([FromQuery(Name = "ID")] int source, HttpContext ctx) =>
{
    /* Configure the response */
    ctx.Response.StatusCode = StatusCodes.Status501NotImplemented;
})
.WithName("telemetry")
.WithOpenApi();

// Point command
app.MapPut("/route", ([FromQuery(Name = "ID")] int source, HttpContext ctx) =>
{
    /* Configure the response */
    ctx.Response.StatusCode = StatusCodes.Status501NotImplemented;
})
.WithName("route")
.WithOpenApi();

// Download image route
app.MapPost("/downloadImage", (HttpContext ctx) =>
{
    /* Configure the response */
    ctx.Response.StatusCode = StatusCodes.Status501NotImplemented;
})
.WithName("Download Image")
.WithOpenApi();

// Payload On Off
app.MapPut("/payloadState", ([FromQuery(Name = "ID")] int source, [FromQuery(Name = "state")] bool state, HttpContext ctx) =>
{
    /* Configure the response */
    ctx.Response.StatusCode = StatusCodes.Status501NotImplemented;
})
.WithName("Payload Power")
.WithOpenApi();

#endregion

app.Run();
