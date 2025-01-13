using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal static class HttpMethodExtensions
  {
    public static readonly HttpMethod Patch = new HttpMethod("PATCH");

    public static bool IsPostOrPatch(HttpMethod method)
    {
      return method == HttpMethod.Post || method == Patch;
    }
  }
}
