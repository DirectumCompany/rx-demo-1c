using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal class HttpRequestMessageBuilder
  {
    public HttpRequestMessageBuilder(HttpMethod method, string url) =>
      this.Result = new HttpRequestMessage(method, url);

    public void AppendBasicAuthHeader(string basicAuthToken) =>
      this.Result.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", basicAuthToken);

    public void AppendContent(object content)
    {
      if (content == null)
        throw new ArgumentNullException(nameof(content));

      this.Result.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
    }

    public HttpRequestMessage Result { get; }
  }
}
