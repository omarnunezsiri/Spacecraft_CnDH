// The Spacecraft C&DH Team licenses this file to you under the MIT license.

using System.Text.Json;
using System.Xml;

namespace JAPI.Handlers;

public class FileHandler
{
    static readonly FileHandler Instance = new FileHandler();

    private FileHandler() { }

    public static FileHandler GetFileHandler() { return Instance; }
    /// <summary>
    /// This method reads from the given file to read in all IP Addresses and Service IDs, puts them in a sictionary and returns them.
    /// </summary>
    /// <param name="fileName">File to read from</param>
    public Dictionary<int, string> ReadIpConfigFile(string fileName)
    {
        Dictionary<int, string> serviceDictionary = new Dictionary<int, string>();
        try
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                while (!sr.EndOfStream)
                {
                    string? line = sr.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                    {
                        line.Trim();
                        string[] serviceInformation = line.Split(',');
                        serviceDictionary.Add(int.Parse(serviceInformation[1]), serviceInformation[0]);
                    }
                }
            }
        }
        catch (FileNotFoundException) { throw new FileNotFoundException($"Could not find the following file attempting to be opened: {fileName}"); }
        catch (IOException) { throw new IOException($"There was an error trying to perform IO operations on the following file: {fileName}"); }
        catch (Exception) { throw; }
        return serviceDictionary;
    }

    /// <summary>
    /// This method reads and deserializes the Telemetry data from a JSON file
    /// </summary>
    /// <param name="fileName">File to read from</param>
    public void ReadTelemtryData(string fileName)
    {
        if (File.Exists(fileName))
        {
            try
            {
                string jsonData = File.ReadAllText(fileName);
                Telemetry telemetryData = JsonSerializer.Deserialize<Telemetry>(jsonData);
            }
            catch (JsonException ex) { Console.WriteLine("Error deserializing JSON: " + ex.Message); }
        }
        else { Console.WriteLine("JSON file not found"); }
    }

    /// <summary>
    /// This method writes and serializes the Telemetry data to a JSON file
    /// </summary>
    /// <param name="fileName">File to read from</param>
    public void WriteTelemetryData(string fileName, Telemetry telemetryData)
    {
        try
        {
            string jsonData = JsonSerializer.Serialize<Telemetry>(telemetryData);
            File.WriteAllText(fileName, jsonData);
        }
        catch (IOException) { throw new IOException($"There was an error trying to perform IO operations on the following file: {fileName}"); }
        catch (JsonException) { throw new JsonException($"There was an error trying to serialize the telemetry data to JSON"); }
        catch (Exception) { throw; }
    }
}
