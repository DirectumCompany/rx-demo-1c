using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http
{
  public class HttpMethodHelper
  {
    private const string PatchMethodName = "PATCH";

    public static HttpMethod CreatePatch => new HttpMethod(PatchMethodName);

    internal static string ConvertToString(HttpMethod method)
     => method.Method.ToUpperInvariant();

    internal static bool MethodRequiresContent(HttpMethod method)
      => method == HttpMethod.Post || ConvertToString(method) == PatchMethodName;
  }
}
