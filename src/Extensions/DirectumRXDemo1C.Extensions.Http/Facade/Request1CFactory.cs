using DirectumRXDemo1C.Extensions.Http.Internal;
using System;
using System.Net.Http;

namespace DirectumRXDemo1C.Extensions.Http
{
  public static class Request1CFactory
  {
    public static Request1C CreateGet(string url)
      => CreateInternal(HttpMethod.Get, url, null);

    public static Request1C CreatePost(string url, object content)
      => CreateInternal(HttpMethod.Get, url, content);

    public static Request1C CreatePatch(string url, object content)
      => CreateInternal(HttpMethod.Get, url, content);

    #region private 

    private static Request1C CreateInternal(HttpMethod method, string url, object content)
    {
      ValidateParameters(method, content);
      return new Request1C(method, url, content);
    }

    private static void ValidateParameters(HttpMethod method, object content)
    {
      if (method == HttpMethod.Get && content != null)
        throw new ArgumentException("Http method GET shouldn't have a content");

      if (HttpMethodHelper.MethodRequiresContent(method) && content == null)
        throw new ArgumentException($"Http method \"{HttpMethodHelper.ConvertToString(method)}\" must have a content");
    }    

    #endregion 
  }
}
