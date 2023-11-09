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

    private Dictionary<int, string> UriValues;

    public void SetUriValues(Dictionary<int, string> input)
    {
        UriValues = input;
    }

    private const int SpaceUpDown = 3;

    #region HttpRequests
    public async Task SendRawData(HttpContent ctx)
    {
        // Uri
        string apiUrl = UriValues[SpaceUpDown];

        // Create post
#if DEBUG
        HttpResponseMessage response = new HttpResponseMessage();
        response.StatusCode = HttpStatusCode.OK;
        response.Content = new StringContent("No Content");
#else
        HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, ctx);
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

    }
    #endregion
}
