namespace JAPI.Handlers
{
    public class Telemetry
    {
        public int Sample { get; set; }

        public static Telemetry Instance { get; } = new Telemetry();
    }
}
