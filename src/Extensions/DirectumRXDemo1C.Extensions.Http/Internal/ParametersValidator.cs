using System;
using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal static class ParametersValidator
  {
    public static void ValidateRequestMethod(RequestMethod method)
    {
      if (!Enum.IsDefined(typeof(RequestMethod), method))
        throw new ArgumentOutOfRangeException(nameof(method));
    }

    public static void ValidateCredentials(string login, string password)
    {
      if (login == null) throw new ArgumentNullException(nameof(login));
      if (password == null) throw new ArgumentNullException(nameof(password));
    }

    public static void ValidateContent(HttpMethod method, object content)
    {
      if (method == HttpMethod.Get && content != null)
        throw new ArgumentException($"Http method \"{HttpMethodToUpper(method)}\" shouldn't have a content");
    }

    private static string HttpMethodToUpper(HttpMethod method) => method.ToString().ToUpperInvariant();
  }
}
