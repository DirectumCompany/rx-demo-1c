using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal class HttpRequestMessageBuilder
  {
    public HttpRequestMessageBuilder(HttpMethod method, string url, string login, string password)
    {
      this.Result = new HttpRequestMessage(method, url);
      var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));
      this.Result.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", basicAuth);
    }

    public void AppendContent(object content)
    {
      if (content == null)
        throw new ArgumentNullException(nameof(content));

      this.Result.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
    }

    public HttpRequestMessage Result { get; }
  }
}
