using System.Net.Http;
using DirectumRXDemo1C.Extensions.Http.Internal;

namespace DirectumRXDemo1C.Extensions.Http
{
  public class Request
  {
    private readonly HttpMethod method;
    private readonly string url;

    private Request(HttpMethod method, string url)
    {
      this.method = method;
      this.url = url;
    }

    public void Invoke(object content = null)
    {
      ParametersValidator.Invoke(method, content);

      var response = HttpClientProvider.Get().SendAsync(this.CreateRequest(content)).Result;
      response.EnsureSuccessStatusCode();

      this.ResponseContent = response.Content.ReadAsStringAsync().Result;
    }

    private HttpRequestMessage CreateRequest(object content)
    {
      var requestBuilder = new HttpRequestMessageBuilder(method, url);
      if (HttpMethodExtensions.IsPostOrPatch(method))
        requestBuilder.AppendContent(content);

      return requestBuilder.Result;
    }

    public string ResponseContent { get; private set; }

    public static Request Create(RequestMethod method, string url)
    {
      var httpMethod = new HttpMethod(method.ToString().ToUpperInvariant());
      return new Request(httpMethod, url);
    }
  }
}
 