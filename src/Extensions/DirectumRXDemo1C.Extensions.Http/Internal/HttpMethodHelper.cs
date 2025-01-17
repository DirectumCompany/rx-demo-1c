using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal static class HttpMethodHelper
  {
    private static readonly HttpMethod Patch = new HttpMethod("PATCH");

    public static HttpMethod GetPatch() => Patch;

    public static bool IsContentRequired(HttpMethod method) => 
      method == Patch;
  }
}