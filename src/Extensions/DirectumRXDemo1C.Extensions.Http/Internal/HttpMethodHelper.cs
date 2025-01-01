using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal class HttpMethodHelper
  {
    internal static string ConvertToString(HttpMethod method)
     => method.Method.ToUpperInvariant();

    internal static bool MethodRequiresContent(HttpMethod method)
      => method == HttpMethod.Post || ConvertToString(method) == "PATCH";
  }
}
