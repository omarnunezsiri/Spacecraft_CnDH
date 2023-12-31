// The Spacecraft C&DH Team licenses this file to you under the MIT license.

using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
namespace JAPI.Handlers;

public class HttpRequestHandler : Controller
{
    private readonly HttpClient _httpClient;

    public HttpRequestHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // All the ID's and IP addresses of the various services
    private Dictionary<int, string> UriValues;

    /// <summary>
    /// Setting the Uri values
    /// </summary>
    /// <param name="input"></param>
    public void SetUriValues(Dictionary<int, string> input)
    {
        UriValues = input;
    }

    public Dictionary<int, string> GetUriValues()
    {
        return UriValues;
    }

    // Id of the space uplink/downlink
    private const int SpaceUpDown = 3;
    // Id of the space Payload
    private const int SpacePayload = 2;
    // port number
    private const string portNumber = ":8080";
    // protocol
    private const string start = "http://";

    #region HttpRequests
    /// <summary>
    /// function for sending rawData recieved to uplink/downlink
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> SendRawData(HttpContent ctx)
    {
        // Uri
        string apiUrl = start + UriValues[SpaceUpDown] + portNumber + "/C&DH_Receive";

        // Create post
        HttpResponseMessage response = new HttpResponseMessage();
        try
        {
#if DEBUG
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("No Content");
#else
            response = await _httpClient.PostAsync(apiUrl, ctx).ConfigureAwait(true);
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return response;
        }
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            Console.WriteLine($"API Response: {responseContent}");
        }
        else
        {
            Console.WriteLine($"API Request failed with status code: {response.StatusCode}");
        }
        return response;
    }
    public async Task<HttpResponseMessage> TogglePayload(bool state)
    {
        // Uri
        string apiUrl = start + UriValues[SpacePayload] + portNumber + "/payloadState?state=" + state.ToString().ToLowerInvariant();
        // Create post
        HttpResponseMessage response = new HttpResponseMessage();
        try
        {
#if DEBUG
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("No Content");
#else
            response = await _httpClient.PutAsync(apiUrl, null).ConfigureAwait(true);
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return response;
        }

        TelemetryHandler telem = TelemetryHandler.Instance();

        // recieve response
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            Console.WriteLine($"API Response: {responseContent}");
            telem.GetTelemetry().status.payloadPower = state;
        }
        else
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.MethodNotAllowed:
                    Console.WriteLine("Payload already in state\n");
                    telem.GetTelemetry().status.payloadPower = state;
                    break;
                case HttpStatusCode.BadRequest:
                    Console.WriteLine("Payload responded Bad Request Not updating telemtry\n");
                    break;
                default:
                    Console.WriteLine("Unknown Error\n");
                    break;
            }
            Console.WriteLine($"API Request failed with status code: {response.StatusCode}");
        }
        return response;
    }

    public async Task<HttpResponseMessage> SendPackagedData(Telemetry telemetry, int ID)
    {
        // Uri
        StringContent requestContent;
        string apiUrl = start + UriValues[SpaceUpDown] + portNumber + "/C&DH_Receive";
        Console.WriteLine(apiUrl);
        try
        {
            var requestData = new Packet();
            requestData.verb = "PUT";
            requestData.uri = UriValues[ID] + ":8080/telemetry";
            requestData.content = telemetry;
            var sendData = JsonSerializer.Serialize(requestData);
            requestContent = new StringContent(sendData, Encoding.UTF8, "application/json");
            string requestConverted = requestContent.ReadAsStringAsync().Result;
#if DEBUG
            Console.WriteLine($"requestConverted in DEBUG\n{requestConverted}");
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        HttpResponseMessage response = new HttpResponseMessage();
        try
        {
#if DEBUG
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("No Content");
#else
            response = await _httpClient.PostAsync(apiUrl, requestContent).ConfigureAwait(true);

#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return response;
        }
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            Console.WriteLine($"API Response: {responseContent}");
        }
        else
        {
            Console.WriteLine($"API Request failed with status code: {response.StatusCode}");
        }
        return response;

    }

    /// <summary>
    /// Request the status of the link from the space uplink/downlink and return the bool recieved
    /// </summary>
    /// <returns></returns>
    public async Task<bool> RequestLinkStatus()
    {
        // Uri
        string apiUrl = start + UriValues[SpaceUpDown] + portNumber + "/GroundConnection";

        // Create response variable
        HttpResponseMessage response = new HttpResponseMessage();
        string responseContent;
        // status variable
        bool status = false;
        try
        {
#if DEBUG
            response.StatusCode = HttpStatusCode.OK;
            // create dummy data
            var jsonObject = new { status = true };
            var jsonContent = JsonSerializer.Serialize(jsonObject);
            response.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#else
            // send request & read content as string
            response = await _httpClient.GetAsync(apiUrl).ConfigureAwait(true);
            responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return status;
        }
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"API Success Response: {responseContent}");
            JsonDocument jsonDocument = JsonDocument.Parse(responseContent);
            status = jsonDocument.RootElement.GetProperty("status").GetBoolean();
        }
        else
        {
            Console.WriteLine($"API Failed Response: {responseContent}");
        }
        return status;
    }
    #endregion
}

public class Packet
{
    public string verb { get; set; }
    public string uri { get; set; }
    public object content { get; set; }
}
