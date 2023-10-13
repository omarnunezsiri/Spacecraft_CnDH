// The Spacecraft C&DH Team licenses this file to you under the MIT license.

using JAPI.Handlers;

#region Setup
//This is where any setup code will go.
FileHandler fileHandler = FileHandler.GetFileHandler();
//TODO: Implement this ReadfIpConfigFile method
fileHandler.ReadIpConfigFile("ips.cfg");

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
#endregion

app.Run();
