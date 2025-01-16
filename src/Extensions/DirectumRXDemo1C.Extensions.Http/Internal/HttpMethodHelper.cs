using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal static class HttpMethodHelper
  {
    private static readonly HttpMethod patch = new HttpMethod("PATCH");

    public static HttpMethod GetPatch() => patch;

    public static bool IsContentRequired(HttpMethod method) => 
      method == HttpMethod.Post || method == patch;
  }
}