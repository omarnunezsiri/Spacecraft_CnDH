// The Spacecraft C&DH Team licenses this file to you under the MIT license.

namespace JAPI.Handlers;

public class TelemetryHandler
{
    private readonly Telemetry telemetry = Telemetry.Instance;

    public object GetTelemetry()
    {
        telemetry.Sample = 50;
        return telemetry;
    }

    /* Simulation */
}
