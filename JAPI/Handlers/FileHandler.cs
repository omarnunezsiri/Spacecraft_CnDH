// The Spacecraft C&DH Team licenses this file to you under the MIT license.

namespace JAPI.Handlers;

public class FileHandler
{
    static readonly FileHandler Instance = new FileHandler();

    private FileHandler() { }

    public static FileHandler GetFileHandler() { return Instance; }

    //Me and hayden will both be implemetning in this file. So here are some lines to stop merge conflicts

    //***********Talon Code*****
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
        catch (FileNotFoundException) { Console.WriteLine($"Could not find the following file attempting to be opened: {fileName}"); serviceDictionary.Clear(); }
        catch (IOException) { Console.WriteLine($"There was an error trying to perform IO operations on the following file: {fileName}"); serviceDictionary.Clear(); }
        catch (Exception) { Console.WriteLine("Something has went wrong with the ReadIpConfigFile method in FileHandler."); serviceDictionary.Clear(); }
        return serviceDictionary;
    }
    //**************************

    //***********Hayden Code*****

    //**************************
}
