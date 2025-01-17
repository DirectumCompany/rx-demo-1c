using System;
using System.Net.Http;
using System.Text;
using DirectumRXDemo1C.Extensions.Http.Internal;

namespace DirectumRXDemo1C.Extensions.Http
{
  public class Request
  {
    #region ctor

    private readonly HttpMethod httpMethod;
    private readonly HttpRequestMessageBuilder requestMessageBuilder;

    private Request(HttpMethod method, string url)
    {
      this.httpMethod = method;
      this.requestMessageBuilder = new HttpRequestMessageBuilder(method, url);
    }

    #endregion

    public void UseBasicAuth(string login, string password)
    {
      ParametersValidator.ValidateCredentials(login, password);

      var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));
      this.requestMessageBuilder.AppendBasicAuthHeader(token);
    }

    public void Invoke(object content = null)
    {
      ParametersValidator.ValidateContent(this.httpMethod, content);

      if (HttpMethodHelper.IsContentRequired(this.httpMethod))
        this.requestMessageBuilder.AppendContent(content);

      var response = HttpClientProvider.Get().SendAsync(this.requestMessageBuilder.Result).Result;
      response.EnsureSuccessStatusCode();

      this.ResponseContent = response.Content.ReadAsStringAsync().Result;
    }

    public string ResponseContent { get; private set; }

    #region fabric method

    public static Request Create(RequestMethod method, string url)
    {
      ParametersValidator.ValidateRequestMethod(method);

      var httpMethod = new HttpMethod(method.ToString().ToUpperInvariant());
      return new Request(httpMethod, url);
    }

    #endregion
  }
}
