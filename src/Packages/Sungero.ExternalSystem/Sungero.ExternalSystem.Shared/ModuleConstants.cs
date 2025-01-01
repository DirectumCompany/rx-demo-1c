using System;
using Sungero.Core;

namespace Sungero.ExternalSystem.Constants
{
  public static class Module
  {
    /// <summary>
    /// Параметры доступа к 1С.
    /// </summary>
    public static class ConnectionParamNames
    {
      /// <summary>
      /// Адрес веб-сервера.
      /// </summary>
      [Sungero.Core.Public]
      public const string ServiceUrl1C = "1CServiceAddress";
      
      /// <summary>
      /// Идентификатор системы.
      /// </summary>
      [Sungero.Core.Public]
      public const string SystemId = "1CSystemId";
    }
  }
}