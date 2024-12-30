using System;
using System.Net.Http;
using System.Text;
using DirectumRXDemo1C.Extensions.Http.Internal;

namespace DirectumRXDemo1C.Extensions.Http
{
  public class Request1C
  {
    private HttpMethod method;
    private readonly string url;
    private readonly object content;

    internal Request1C(HttpMethod method, string url, object content)
    {
      this.method = method;
      this.url = url;
      this.content = content;
    }

    public void Execute()
    {
      var response = HttpClientLocator.GetOrCreate().SendAsync(this.CreateRequest()).Result;
      response.EnsureSuccessStatusCode();

      this.Result = response.Content.ReadAsStringAsync().Result;
    }

    private HttpRequestMessage CreateRequest()
    {
      var requestBuilder = new HttpRequestMessageBuilder(method, url);
      if (HttpMethodHelper.MethodRequiresContent(method))
        requestBuilder.AppendContent(content);

      return requestBuilder.Result;
    }

    public string Result { get; private set; }
  }
}
