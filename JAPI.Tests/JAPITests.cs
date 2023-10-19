// The Spacecraft C&DH Team licenses this file to you under the MIT license.

namespace JAPI.Tests;

[TestClass]
public class JAPITests
{
    /*
    *  Test cases for the J API
    */

    [TestMethod]

}

[TestClass]
public class FileIOTests
{
    /*
    *  File IO test cases
    */

    private string? _ipCfgFile;
    private string? _wrongIpFormatFile;

    [TestInitialize]
    public void TestInitialize()
    {
        // Calculate the relative path to go up two directories
        string relativePath = "..\\..\\..\\assets";

        // Combine the relative path with the test assembly's location
        string pathToWorkingDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));

        // Set the current working directory to the calculated path
        Environment.CurrentDirectory = pathToWorkingDirectory;

        _ipCfgFile = "ips.cfg"; /* Used to test the ReadIpConfigFile method */
        _wrongIpFormatFile = "wrongIps.cfg"; /* Used to test wrong format */
    }

    [TestMethod]
    public void FILE001_GetFileHandler__TypeIsFileHandler()
    {
        // Arrange and Act
        object fileHandler = FileHandler.GetFileHandler();

        // Assert
        Assert.IsNotNull(fileHandler, "FileHandler is null.");
        Assert.IsTrue(fileHandler is FileHandler, "Actual is not of type FileHandler.");
    }

    [TestMethod]
    public void FILE002_ReadIpConfigFile_CorrectFormat_IPsInDictionary()
    {
        // Arrange
        /* Initialize expected IPs*/
        const string FirstService = "192.10.10.1";
        const string SecondService = "192.10.11.2";

        FileHandler fileHandler = FileHandler.GetFileHandler(); // retrieve the FileHandler singleton
        Dictionary<int, string> serviceDictionary = new Dictionary<int, string>();

        // Act
        if (!string.IsNullOrEmpty(_ipCfgFile))
        {
            serviceDictionary = fileHandler.ReadIpConfigFile(_ipCfgFile);
        }

        // Assert
        Assert.IsTrue(serviceDictionary[1] == FirstService && serviceDictionary[2] == SecondService);
    }

    [TestMethod]
    public void FILE003_ReadIpConfigFile_FileNotFound_ExceptionThrown()
    {
        // Arrange
        FileHandler fileHandler = FileHandler.GetFileHandler();

        // Act and Assert
        Assert.ThrowsException<FileNotFoundException>(() =>
        {
            fileHandler.ReadIpConfigFile("JaVaAscIi.txt"); // Call the method that should throw the exception
        });
    }

    [TestMethod]
    public void FILE004_ReadIpConfigFile_WrongFormat_ExceptionThrown()
    {
        // Arrange
        FileHandler fileHandler = FileHandler.GetFileHandler();

        // Act and Assert
        Assert.ThrowsException<IndexOutOfRangeException>(() =>
        {
            if (!string.IsNullOrEmpty(_wrongIpFormatFile))
            {
                fileHandler.ReadIpConfigFile(_wrongIpFormatFile); // Call the method that should throw the exception
            }
        });
    }
}
