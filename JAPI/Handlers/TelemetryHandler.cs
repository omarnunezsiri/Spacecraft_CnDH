using System.Text.Json;

namespace JAPI.Handlers
{
    public class TelemetryHandler
    {
        private readonly Telemetry telemetry = Telemetry.Instance;
        public string GetTelemetry()
        {
            telemetry.Sample = 50;
            return JsonSerializer.Serialize(telemetry);
        }

        /* Simulation */
    }
}
