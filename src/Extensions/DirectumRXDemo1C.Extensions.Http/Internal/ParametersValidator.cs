using System;
using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal static class ParametersValidator
  {
    public static void Invoke(HttpMethod method, object content)
    {
      if (method == HttpMethod.Get && content != null)
        throw new ArgumentException($"Http method \"{HttpMethodHelper.ConvertToString(method)}\" shouldn't have a content");

      if (HttpMethodHelper.MethodRequiresContent(method) && content == null)
        throw new ArgumentException($"Http method \"{HttpMethodHelper.ConvertToString(method)}\" must have a content");
    }
  }
}
