// The Spacecraft C&DH Team licenses this file to you under the MIT license.

using System.Text.Json;

namespace JAPI.Tests;

[TestClass]
public class JAPITests
{
    /*
    *  Test cases for the J API
    */

    private TestServer? _testServer;
    private HttpClient? _testClient;

    [TestInitialize]
    public void TestInitialize()
    {
        _testServer = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());
        _testClient = _testServer.CreateClient();
    }

    [TestMethod]
    public async Task JAPI001_GetResource_NotFound_Returns404()
    {
        if (_testClient is not null)
        {
            // Arrange and Act
            var response = await _testClient.GetAsync("/pong").ConfigureAwait(true);

            // Arrange
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Error code received is not expected 404");
        }
    }
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
        string assetsPath = Path.Combine("..", "..", "..", "assets");
        string absolutePathToAssets = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assetsPath);

        // Set the current working directory to the specified absolute path
        Environment.CurrentDirectory = absolutePathToAssets;

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

    [TestMethod]
    public void FILE005_ReadTelemetryData_ValidFile_ReturnsTelemetryData()
    {
        //Arrange
        FileHandler fileHandler = FileHandler.GetFileHandler();

        string fileName = "testfile.json";
        Telemetry expectedTelemetryData = new Telemetry
        {
            coordinate = new Coordinate(2, 3, 4),
            rotation = new Rotation(5, 6, 7),
            fuel = 0.75f,
            temp = 28.0f,
            status = new Status(true, true, false, 69.0f)
        };

        //Create a test file with known JSON data
        string jsonData = JsonSerializer.Serialize(expectedTelemetryData);
        File.WriteAllText(fileName, jsonData);

        //Act
        Telemetry? actualTelemetryData = fileHandler.ReadTelemetryData(fileName);

        //Assert
        if (actualTelemetryData != null)
        {
            Assert.AreEqual(expectedTelemetryData, actualTelemetryData);
        }

    }

    [TestMethod]
    public void FILE006_WriteTelemetryData_ValidData_WritesToFile()
    {
        //Arranage
        FileHandler fileHandler = FileHandler.GetFileHandler();

        string fileName = "testfile.json";
        Telemetry telemetryData = new Telemetry
        {
            coordinate = new Coordinate(2, 3, 4),
            rotation = new Rotation(5, 6, 7),
            fuel = 0.75f,
            temp = 28.0f,
            status = new Status(true, true, false, 69.0f)
        };

        //Act
        fileHandler.WriteTelemetryData(fileName, telemetryData);

        //Assert
        string jsonData = File.ReadAllText(fileName);
        Telemetry? deserializedData = JsonSerializer.Deserialize<Telemetry?>(jsonData);
        if (deserializedData != null)
        {
            Assert.AreEqual(telemetryData, deserializedData);
        }
    }
}
