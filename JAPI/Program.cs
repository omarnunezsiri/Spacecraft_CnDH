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
app.MapGet("/", () => {
    TelemetryHandler telemetryHandler = new TelemetryHandler();
    string jsonPayload = telemetryHandler.GetTelemetry();

    return Results.Ok(jsonPayload);
})
.WithName("Root")
.WithOpenApi();
#endregion

app.Run();