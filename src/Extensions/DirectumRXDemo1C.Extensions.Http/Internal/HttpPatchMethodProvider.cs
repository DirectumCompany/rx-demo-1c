using System;
using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http.Internal
{
  internal static class HttpPatchMethodProvider
  {
    private static readonly HttpMethod _patch = new HttpMethod("PATCH");

    public static HttpMethod Get() => _patch;
  }
}