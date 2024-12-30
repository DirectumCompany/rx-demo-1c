using System;
using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal static class HttpClientLocator
  {    
    public static HttpClient GetOrCreate() 
      => httpClient ?? (httpClient = CreateClient());

    #region Private

    private static HttpClient httpClient;

    private static HttpClient CreateClient()
    {
      var httpClientHandler = new HttpClientHandler
      {
        ServerCertificateCustomValidationCallback =
        (message, cert, chain, sslPolicyErrors) => true
      };
      httpClient = new HttpClient(httpClientHandler)
      {
        Timeout = TimeSpan.FromMinutes(5)
      };

      return httpClient;
    }

    #endregion
  }
}
