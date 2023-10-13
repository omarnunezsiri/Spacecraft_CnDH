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
    /// This method reads from the given file to read in all IP Addresses and Service IDs
    /// </summary>
    /// <param name="fileName">File to read from</param>
    public void ReadIpConfigFile(string fileName)
    {

    }
    //**************************

    //***********Hayden Code*****

    //**************************
}
