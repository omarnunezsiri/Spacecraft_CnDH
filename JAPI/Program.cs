using JAPI.Handlers;

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
  
    return object;
})
.WithName("Root")
.WithOpenApi();
#endregion

app.Run();