using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal static class HttpMethodHelper
  {
    public static string ConvertToString(HttpMethod method)
      => method.Method.ToUpperInvariant();

    public static bool MethodRequiresContent(HttpMethod method)
      => method == HttpMethod.Post || ConvertToString(method) == "PATCH";
  }
}
