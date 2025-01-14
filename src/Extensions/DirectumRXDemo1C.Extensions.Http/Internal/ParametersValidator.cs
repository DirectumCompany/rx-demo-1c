using System;
using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal static class ParametersValidator
  {
    public static void Invoke(HttpMethod method, object content)
    {
      if (method == HttpMethod.Get && content != null)
        throw new ArgumentException($"Http method \"{HttpMethodToUpper(method)}\" shouldn't have a content");

      if (HttpMethodTools.IsContentRequired(method) && content == null)
        throw new ArgumentException($"Http method \"{HttpMethodToUpper(method)}\" must have a content");
    }

    private static string HttpMethodToUpper(HttpMethod method) => method.ToString().ToUpperInvariant();
  }
}
