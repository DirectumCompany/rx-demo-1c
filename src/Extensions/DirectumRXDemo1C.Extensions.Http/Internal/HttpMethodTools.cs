using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal static class HttpMethodTools
  {
    public static readonly HttpMethod Patch = new HttpMethod("PATCH");

    public static bool IsContentRequired(HttpMethod method) => method == HttpMethod.Post || method == Patch;
  }
}
