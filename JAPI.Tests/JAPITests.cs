// The Spacecraft C&DH Team licenses this file to you under the MIT license.

using System.Text;
using Newtonsoft.Json;

namespace JAPI.Tests;

[TestClass]
public class JAPITests
{
    /*
    *  Test cases for the J API
    */

    private TestServer? _testServer;
    private HttpClient? _testClient;
    private HttpRequestHandler? _httpRequestHandler;

    [TestInitialize]
    public void TestInitialize()
    {
        _testServer = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());
        _testClient = _testServer.CreateClient();
        _httpRequestHandler = new HttpRequestHandler(_testClient);
        Dictionary<int, string> temp = new Dictionary<int, string>();
        temp.Add(3, "xxx.xxx.xxx.x");
        _httpRequestHandler.SetUriValues(temp);
        Startup.SendHandler = _httpRequestHandler;
        TelemetryHandler.Instance().GetTelemetry().status.payloadPower = false;
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

    [TestMethod]
    public async Task JAPI002_downloadImage_NoContent_Returns204()
    {
        if (_testClient is not null)
        {
            // Arrange and Act
            var requestData = new TestPacketData
            {
                verb = "POST",
                uri = "xxx.xxx.xxx.x",
                data = new TestRawData
                {
                    raw = "0x12382181828122",
                    sequence = 1
                }

            };
            string jsonBody = JsonConvert.SerializeObject(requestData);
            HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await _testClient.PostAsync("/downloadImage", content).ConfigureAwait(true);

            // Arrange
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode, "Error code received is not expected 204");
        }
    }

    [TestMethod]
    public async Task JAPI003_PointRoute_MethodNotAllowed_Returns405()
    {
        if (_testClient is not null)
        {
            //Arrange and Act
            TelemetryHandler handler = TelemetryHandler.Instance();
            handler.GetTelemetry().status.chargeStatus = true;
            var requestData = new Telemetry
            {
                coordinate = new Coordinate(1, 2.5f, 3),
                rotation = new Rotation(0, 0.8f, 1.2f)
            };
            string jsonBody = JsonConvert.SerializeObject(requestData);
            HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await _testClient.PutAsync("/point?ID=1", content).ConfigureAwait(true);

            //Arrange
            Assert.AreEqual(HttpStatusCode.MethodNotAllowed, response.StatusCode, "Error code received is not expected 405");
        }
    }

    [TestMethod]
    public async Task JAPI004_PointRoute_BadRequest_Return400()
    {
        if (_testClient is not null)
        {
            //Arrange and Act
            TelemetryHandler handler = TelemetryHandler.Instance();
            handler.GetTelemetry().status.chargeStatus = false;
            var requestData = new Telemetry
            {
                coordinate = null,
                rotation = null
            };
            string jsonBody = JsonConvert.SerializeObject(requestData);
            HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await _testClient.PutAsync("/point?ID=1", content).ConfigureAwait(true);

            //Arrange
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Error code received is not expected 400");
        }
    }

    [TestMethod]
    public async Task JAPI005_PointRoute_OK_Return200()
    {
        if (_testClient is not null)
        {
            //Arrange and Act
            TelemetryHandler handler = TelemetryHandler.Instance();
            handler.GetTelemetry().status.chargeStatus = false;
            var requestData = new Telemetry
            {
                coordinate = new Coordinate(1, 2.5f, 3),
                rotation = new Rotation(0, 0.8f, 1.2f)
            };
            string jsonBody = JsonConvert.SerializeObject(requestData);
            HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await _testClient.PutAsync("/point?ID=1", content).ConfigureAwait(true);

            //Arrange
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Error code received is not expected 200");
        }
    }
    [TestMethod]
    public async Task JAPI006_PayloadToggle_OK_Return200()
    {
        if (_testClient is not null)
        {
            // Arrange and Act
            var response = await _testClient.PutAsync("/payloadState?ID=3&state=true", null).ConfigureAwait(true);

            // Arrange
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Error code received is not expected 200");
        }
    }
    [TestMethod]
    public async Task JAPI007_Payload_BadRequest_Return400()
    {
        if (_testClient is not null)
        {
            // Arrange and Act
            var response = await _testClient.PutAsync("/payloadState?ID=1000&state=true", null).ConfigureAwait(true);

            // Arrange
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Error code received is not expected 400");
        }
    }
    [TestMethod]
    public async Task JAPI008_Payload_NotAllowed_Return405()
    {
        if (_testClient is not null)
        {
            // Arrange and Act
            var response = await _testClient.PutAsync("/payloadState?ID=3&state=false", null).ConfigureAwait(true);

            // Arrange
            Assert.AreEqual(HttpStatusCode.MethodNotAllowed, response.StatusCode, "Error code received is not expected 405");
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
        string jsonData = System.Text.Json.JsonSerializer.Serialize(expectedTelemetryData);
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
        Telemetry? deserializedData = System.Text.Json.JsonSerializer.Deserialize<Telemetry?>(jsonData);
        if (deserializedData != null)
        {
            Assert.AreEqual(telemetryData, deserializedData);
        }
    }
}
[TestClass]
public class DataTests
{

    Telemetry telemetryData;
    readonly Random _random = new();

    public void Setup(Coordinate? coord, Rotation? rot, float fuel, float temp, Status status)
    {
        telemetryData = new Telemetry();
        telemetryData.coordinate = coord;
        telemetryData.rotation = rot;
        telemetryData.fuel = fuel;
        telemetryData.temp = temp;
        telemetryData.status = status;
    }

    [TestMethod]
    public void DATA001_NullCoordinateClass_FalseIsReturned()
    {
        //Arrange
        bool result, expectedResult = false;
        Setup(null, new Rotation(2, 3, 4), 50, 32, new Status(true, true, true, 50));
        //Act
        result = telemetryData.UpdateShipDirection(1, 2, 3, 4, 5, 6);

        //Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void DATA002_NullRotationClass_FalseIsReturned()
    {
        //Arrange
        bool result, expectedResult = false;
        Setup(new Coordinate(1, 2, 3), null, 50, 32, new Status(true, true, true, 50));
        //Act
        result = telemetryData.UpdateShipDirection(1, 2, 3, 4, 5, 6);

        //Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void DATA003_ValidInput_TrueIsReturned()
    {
        //Arrange
        bool result, expectedResult = true;
        Setup(new Coordinate(1, 2, 3), new Rotation(4, 5, 6), 50, 32, new Status(true, true, true, 50));
        //Act
        result = telemetryData.UpdateShipDirection(7, 8, 9, 10, 11, 12);

        //Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void DATA004_ValidInput_CoordinatesAreUpdated()
    {
        //Arrange
        Coordinate expectedResult = new Coordinate(7, 8, 9);
        Setup(new Coordinate(1, 2, 3), new Rotation(4, 5, 6), 50, 32, new Status(true, true, true, 50));

        //Act
        telemetryData.UpdateShipDirection(7, 8, 9, 10, 11, 12);

        //Assert
        Assert.AreEqual(expectedResult, telemetryData.coordinate);
    }

    [TestMethod]
    public void DATA005_ValidInput_RotationIsUpdated()
    {
        //Arrange
        Rotation expectedResult = new Rotation(10, 11, 12);
        Setup(new Coordinate(1, 2, 3), new Rotation(4, 5, 6), 50, 32, new Status(true, true, true, 50));

        //Act
        telemetryData.UpdateShipDirection(7, 8, 9, 10, 11, 12);

        //Assert
        Assert.AreEqual(expectedResult, telemetryData.rotation);
    }

    [TestMethod]
    public void DATA006_SetFuel_OutOfInnerBound_FuelIsSetToMinValue()
    {
        // Arrange
        float expectedFuel = Telemetry.MinFuel;
        Setup(new Coordinate(1, 2, 3), new Rotation(4, 5, 6), 0, 32, new Status(true, true, true, 50));

        // Act
        telemetryData.fuel = Telemetry.MinFuel - (float)(_random.NextDouble() * (Telemetry.MaxFuel - Telemetry.MinFuel) + Telemetry.MinFuel); // set fuel to a value out of bounds (lower than the minimum: 0)

        // Assert
        Assert.AreEqual(expectedFuel, telemetryData.fuel);
    }

    [TestMethod]
    public void DATA007_SetFuel_OutOfUpperBound_FuelIsSetToMaxValue()
    {
        // Arrange
        float expectedFuel = Telemetry.MaxFuel;
        Setup(new Coordinate(1, 2, 3), new Rotation(4, 5, 6), 0, 32, new Status(true, true, true, 50));

        // Act
        telemetryData.fuel = Telemetry.MaxFuel + (float)(_random.NextDouble() * (Telemetry.MaxFuel - Telemetry.MinFuel) + Telemetry.MinFuel); // set fuel to a value out of bounds (bigger than the max: 100)

        // Assert
        Assert.AreEqual(expectedFuel, telemetryData.fuel);
    }

    [TestMethod]
    public void DATA008_SetFuel_ValueInBounds_FuelIsSetToVal()
    {
        // Arrange
        float expectedFuel = Telemetry.MinFuel + 3.69f;
        Setup(new Coordinate(1, 2, 3), new Rotation(4, 5, 6), 0, 32, new Status(true, true, true, 50));

        // Act
        telemetryData.fuel = Telemetry.MinFuel + 3.69f; // set fuel to a value in bounds

        // Assert
        Assert.AreEqual(expectedFuel, telemetryData.fuel);
    }

    [TestMethod]
    public void DATA009_SetTemperature_OutOfInnerBound_TempIsSetToMin()
    {
        // Arrange
        float expectedTemp = Telemetry.LowestTemperature;
        Setup(new Coordinate(1, 2, 3), new Rotation(4, 5, 6), 0, 32, new Status(true, true, true, 50));

        // Act
        telemetryData.temp = Telemetry.LowestTemperature - (float)(_random.NextDouble() * (Telemetry.HighestTemperature - Telemetry.LowestTemperature) + Telemetry.LowestTemperature);

        // Assert
        Assert.AreEqual(expectedTemp, telemetryData.temp);
    }

    [TestMethod]
    public void DATA010_SetTemperature_OutOfUpperBound_TempIsSetToMax()
    {
        // Arrange
        float expectedTemp = Telemetry.HighestTemperature;
        Setup(new Coordinate(1, 2, 3), new Rotation(4, 5, 6), 0, 32, new Status(true, true, true, 50));

        // Act
        telemetryData.temp = Telemetry.HighestTemperature + (float)(_random.NextDouble() * (Telemetry.HighestTemperature - Telemetry.LowestTemperature) + Telemetry.LowestTemperature);

        // Assert
        Assert.AreEqual(expectedTemp, telemetryData.temp);
    }

    [TestMethod]
    public void DATA011_SetTemperature_ValueInBounds_TempIsSetToVal()
    {
        // Arrange
        float expectedTemp = Telemetry.HighestTemperature - 0.5f;
        Setup(new Coordinate(1, 2, 3), new Rotation(4, 5, 6), 0, 32, new Status(true, true, true, 50));

        // Act
        telemetryData.temp = Telemetry.HighestTemperature - 0.5f;

        // Assert
        Assert.AreEqual(expectedTemp, telemetryData.temp);
    }

    [TestMethod]
    public void DATA012_SetVoltage_OutLowerOfBound_VoltageIsSetToMin()
    {
        // Arrange
        float expectedVoltage = Status.VoltageMin;
        Setup(new Coordinate(1, 2, 3), new Rotation(4, 5, 6), 0, 32, new Status(true, true, true, 50));

        // Act
        if (telemetryData.status is not null)
        {
            telemetryData.status.voltage = Status.VoltageMin - (float)(_random.NextDouble() * (Status.VoltageMax - Status.VoltageMin) + Status.VoltageMin);

            // Assert
            Assert.AreEqual(expectedVoltage, telemetryData.status.voltage);
        }
        else
        {
            throw new FieldAccessException("Telemetry data status is null.");
        }
    }

    [TestMethod]
    public void DATA013_SetVoltage_OutUpperOfBound_VoltageIsSetToMin()
    {
        // Arrange
        float expectedVoltage = Status.VoltageMax;
        Setup(new Coordinate(1, 2, 3), new Rotation(4, 5, 6), 0, 32, new Status(true, true, true, 50));

        // Act
        if (telemetryData.status is not null)
        {
            telemetryData.status.voltage = Status.VoltageMin + (float)(_random.NextDouble() * (Status.VoltageMax - Status.VoltageMin) + Status.VoltageMin);

            // Assert
            Assert.AreEqual(expectedVoltage, telemetryData.status.voltage);
        }
        else
        {
            throw new FieldAccessException("Telemetry data status is null.");
        }
    }

    [TestMethod]
    public void DATA014_SetVoltage_OnBounds_VoltageIsSetToVal()
    {
        // Arrange
        float expectedVoltage = Status.VoltageMin + 0.6f;
        Setup(new Coordinate(1, 2, 3), new Rotation(4, 5, 6), 0, 32, new Status(true, true, true, 50));

        // Act
        if (telemetryData.status is not null)
        {
            telemetryData.status.voltage = Status.VoltageMin + 0.6f;

            // Assert
            Assert.AreEqual(expectedVoltage, telemetryData.status.voltage);
        }
        else
        {
            throw new FieldAccessException("Telemetry data status is null.");
        }
    }
}

[TestClass]
public class HttpRequestTests
{
    /*
     * Test cases for the HttpRequestHandler
     */

    private TestServer? _testServer;
    private HttpClient? _testClient;
    private HttpRequestHandler _httpRequestHandler;

    [TestInitialize]
    public void TestInitialize()
    {
        _testServer = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());
        _testClient = _testServer.CreateClient();
        _httpRequestHandler = new HttpRequestHandler(_testClient);
        Dictionary<int, string> temp = new Dictionary<int, string>();
        temp.Add(3, "xxx.xxx.xxx.x");
        temp.Add(2, "xxx.xxx.xxx.x");
        _httpRequestHandler.SetUriValues(temp);
    }
    [TestMethod]
    public async Task HttpRequestHandler001_SendRawData_NoContent_ReturnsCode()
    {
        if (_testClient is not null)
        {
            // Arrange and Act
            StringContent content = null;
            HttpResponseMessage response = await _httpRequestHandler.SendRawData(content).ConfigureAwait(true);

            // Assert
#if DEBUG
            Assert.IsTrue(response.IsSuccessStatusCode);
#else
            Assert.IsFalse(response.IsSuccessStatusCode);
#endif
        }
    }
    [TestMethod]
    public async Task HttpRequestHandler002_TogglePayload_true_ReturnsCode()
    {
        if (_testClient is not null)
        {
            // Arrange and Act
            StringContent content = null;
            HttpResponseMessage response = await _httpRequestHandler.TogglePayload(true).ConfigureAwait(true);

            // Assert
#if DEBUG
            Assert.IsTrue(response.IsSuccessStatusCode);
#else
            Assert.IsFalse(response.IsSuccessStatusCode);
#endif
        }

    }

    [TestMethod]
    public async Task HttpRequestHandler003_RequestLinkStatus_NoInput_ReturnsTrue()
    {
        if (_testClient is not null)
        {
            // Arrange and Act
            bool status = await _httpRequestHandler.RequestLinkStatus().ConfigureAwait(true);

            // Assert
#if DEBUG
            Assert.IsTrue(status);
#else
            Assert.IsFalse(status);
#endif

        }
    }

    [TestMethod]
    public async Task HttpRequestHandler002_SendRawData_ReturnsCode()
    {
        //Arrange and Act
        TestRawData data = new TestRawData();
        data.raw = "raw";
        data.sequence = 69;
        string sendData = System.Text.Json.JsonSerializer.Serialize(data);

        var requestContent = new StringContent(sendData, Encoding.UTF8, "application/json");

        int ID = 3;
        HttpResponseMessage response = await _httpRequestHandler.SendPackagedData(requestContent, ID).ConfigureAwait(true);

        //Assert
#if DEBUG
        Assert.IsTrue(response.IsSuccessStatusCode);
#else
        Assert.IsFalse(response.IsSuccessStatusCode);
#endif
    }
}

public class TestPacketData
{
    public string verb;
    public string uri;
    public TestRawData data;
}
public class TestRawData
{
    public string raw;
    public int sequence;
}
