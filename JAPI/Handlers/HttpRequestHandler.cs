// The Spacecraft C&DH Team licenses this file to you under the MIT license.

using System.Net;
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

    // Id of the space uplink/downlink
    private const int SpaceUpDown = 3;

    #region HttpRequests
    /// <summary>
    /// function for sending rawData recieved to uplink/downlink
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> SendRawData(HttpContent ctx)
    {
        // Uri
        string apiUrl = UriValues[SpaceUpDown];

        // Create post
#if DEBUG
        HttpResponseMessage response = new HttpResponseMessage();
        response.StatusCode = HttpStatusCode.OK;
        response.Content = new StringContent("No Content");
#else
        HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, ctx).ConfigureAwait(true);
#endif
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
    #endregion
}
