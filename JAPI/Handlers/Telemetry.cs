// The Spacecraft C&DH Team licenses this file to you under the MIT license.

namespace JAPI.Handlers;

public class Telemetry
{
    public int Sample { get; set; }

    public static Telemetry Instance { get; } = new Telemetry();
}
