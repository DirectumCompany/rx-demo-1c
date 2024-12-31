using System;
using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal static class HttpClientProvider
  {
    public static HttpClient Get() => lazyInstance.Value;

    #region lazy instance

    private static Lazy<HttpClient> lazyInstance = new Lazy<HttpClient>(CreateClient);

    private static HttpClient CreateClient()
    {
      var httpClientHandler = new HttpClientHandler
      {
        ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
      };
      return new HttpClient(httpClientHandler)
      {
        Timeout = TimeSpan.FromMinutes(5)
      };
    }

    #endregion
  }
}
