// The Spacecraft C&DH Team licenses this file to you under the MIT license.

namespace JAPI.Handlers;

public class TelemetryHandler
{
    private static TelemetryHandler instance;
    private readonly Telemetry telemetry = new();

    private TelemetryHandler()
    {
        telemetry.coordinate = new Coordinate(0, 0, 0);
        telemetry.rotation = new Rotation(0, 0, 0);
        telemetry.fuel = 100.0f;
        telemetry.temp = 20.0f;
        telemetry.status = new Status(false, false, false, 100.0f);
    }

    public static TelemetryHandler Instance()
    {
        if (instance == null)
        {
            instance = new TelemetryHandler();
        }
        return instance;
    }

    public Telemetry GetTelemetry()
    {
        return telemetry;
    }

    /* Simulation */
}
