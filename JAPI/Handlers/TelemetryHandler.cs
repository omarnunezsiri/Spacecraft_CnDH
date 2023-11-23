// The Spacecraft C&DH Team licenses this file to you under the MIT license.

using System.Diagnostics;

namespace JAPI.Handlers;

public class TelemetryHandler
{
    /* Class members */
    private static TelemetryHandler? _instance;
    private readonly Telemetry telemetry = new();
    private readonly Mutex _mutex = new Mutex(false);
    private static readonly object _instanceLocker = new();

    /* Utility variables for simulation */
    private readonly Random _random;
    private const int DecreaseValue = 0;
    private const int BreathingTimeInMs = 5;
    private const int IncreaseValue = 2;
    private readonly float[] FuelVariations = { 0.0f, 0.5f, 1.0f, 1.5f };
    private readonly float[] VoltageVariations = { 0.0f, 0.5f };
    private readonly float[] TemperatureVariations = { 0.1f, 0.2f, 0.3f };

    private TelemetryHandler()
    {
        telemetry.coordinate = new Coordinate(0, 0, 0);
        telemetry.rotation = new Rotation(0, 0, 0);
        telemetry.fuel = 100.0f;
        telemetry.temp = 12.0f;
        telemetry.status = new Status(false, false, false, 12.5f);
        _random = new();
    }

    public static TelemetryHandler Instance()
    {
        /* Double-locked idiom for singleton pattern */
        if (_instance == null)
        {
            lock (_instanceLocker)
            {
                _instance ??= new();
            }
        }

        return _instance;
    }

    public Telemetry GetTelemetry()
    {
        return telemetry;
    }

    /* Simulation */
    public void FlyThroughSpace(string fileName, double executeSimulation, FileHandler fh, CancellationToken ct)
    {
        Stopwatch stopwatch = new();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Starting simulation worker...");
        Console.ForegroundColor = ConsoleColor.White;
        if (telemetry.coordinate != null && telemetry.rotation != null && telemetry.status != null)
        {
            stopwatch.Start();

            float newFuel;
            float newTemperature;
            float newVoltage;

            int fuelVariationsSize = FuelVariations.Length;
            int temperatureVariationsSize = TemperatureVariations.Length;
            int voltageVariationsSize = VoltageVariations.Length;

            while (!ct.IsCancellationRequested)
            {
                if (stopwatch.ElapsedMilliseconds >= executeSimulation)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Stopwatch has reached {executeSimulation} milliseconds...executing simulation");
                    Console.ForegroundColor = ConsoleColor.White;

                    /* If the charge status is set to on, the fuel (battery) will go up. Down otherwise. */
                    if (telemetry.status.chargeStatus is true)
                    {
                        newFuel = telemetry.fuel + FuelVariations[_random.Next(0, fuelVariationsSize)];
                    }
                    else
                    {
                        newFuel = telemetry.fuel - FuelVariations[_random.Next(0, fuelVariationsSize)];
                    }

                    /* Utilize a random generator to determine if the temperature and voltage will go down or up */
                    int upOrDownTemp = _random.Next(DecreaseValue, IncreaseValue);
                    int upOrDownVoltage = _random.Next(DecreaseValue, IncreaseValue);

                    if (upOrDownTemp == DecreaseValue)
                    {
                        newTemperature = telemetry.temp - TemperatureVariations[_random.Next(0, temperatureVariationsSize)];
                    }
                    else
                    {
                        newTemperature = telemetry.temp + TemperatureVariations[_random.Next(0, temperatureVariationsSize)];
                    }

                    if (upOrDownVoltage == DecreaseValue)
                    {
                        newVoltage = telemetry.status.voltage - VoltageVariations[_random.Next(0, voltageVariationsSize)];
                    }
                    else
                    {
                        newVoltage = telemetry.status.voltage + VoltageVariations[_random.Next(0, voltageVariationsSize)];
                    }

                    /* Critical section (update telemetry in-memory and to the database (json file) */
                    _mutex.WaitOne();
                    telemetry.fuel = newFuel;
                    telemetry.temp = newTemperature;
                    telemetry.status.voltage = newVoltage;
                    fh.WriteTelemetryData(fileName, telemetry);
                    _mutex.ReleaseMutex();

                    stopwatch.Restart(); // restart the timestep

                    Console.WriteLine($"{telemetry.fuel} ---> {telemetry.temp} ---> {telemetry.status.voltage}");
                }
                else
                {
                    //Console.WriteLine("Timer hasn't reached 30 seconds yet...");
                }

                Thread.Sleep(BreathingTimeInMs); // allow the current worker to have a breathing time (not constantly spinning)
            }

            Debug.WriteLine("Cancellation signal was requested...stopping all simulations");
        }
    }
}
