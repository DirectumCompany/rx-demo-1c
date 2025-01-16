using System;
using System.Net.Http;
using System.Text;
using DirectumRXDemo1C.Extensions.Http.Internal;

namespace DirectumRXDemo1C.Extensions.Http
{
  public class Request
  {
    private readonly HttpMethod httpMethod;
    private readonly HttpRequestMessageBuilder requestMessageBuilder;

    private Request(HttpMethod method, string url)
    {
      httpMethod = method;
      requestMessageBuilder = new HttpRequestMessageBuilder(method, url);
    }

    public void UseBasicAuth(string login, string password)
    {
      var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));
      requestMessageBuilder.AppendBasicAuthHeader(token);
    }
    
    public void Invoke(object content = null)
    {
      ParametersValidator.Invoke(httpMethod, content);

      var response = HttpClientProvider.Get().SendAsync(this.CreateRequest(content)).Result;
      response.EnsureSuccessStatusCode();

      this.ResponseContent = response.Content.ReadAsStringAsync().Result;
    }

    private HttpRequestMessage CreateRequest(object content)
    {
      if (httpMethod == HttpMethod.Post)
        requestMessageBuilder.AppendContent(content);

      return requestMessageBuilder.Result;
    }

    public string ResponseContent { get; private set; }

    public static Request Create(RequestMethod method, string url)
    {
      var httpMethod = new HttpMethod(method.ToString().ToUpperInvariant());
      return new Request(httpMethod, url);
    }
  }
}