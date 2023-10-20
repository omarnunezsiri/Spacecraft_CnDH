// The Spacecraft C&DH Team licenses this file to you under the MIT license.

namespace JAPI.Handlers;

public class TelemetryHandler
{
    private readonly Telemetry telemetry = new();

    public object GetTelemetry()
    {
        return telemetry;
    }

    /* Simulation */
}
