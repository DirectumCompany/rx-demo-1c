using System;
using Sungero.Core;

namespace Sungero.ExternalSystem.Constants
{
  public static class Module
  {
    /// <summary>
    /// Локальный адрес веб-сервера по умолчанию.
    /// </summary>
    public const string DefaultServiceUrl1C = "https://localhost/1C_Accounting";
    
    /// <summary>
    /// Параметры доступа к 1С.
    /// </summary>
    public static class ConnectionParamNames
    {
      /// <summary>
      /// Адрес веб-сервера.
      /// </summary>      
      public const string ServiceUrl1C = "1CServiceAddress";
      
      /// <summary>
      /// Идентификатор системы.
      /// </summary>      
      public const string SystemId = "1CSystemId";
      
      /// <summary>
      /// Логин пользователя.
      /// </summary>
      public const string Login = "1СUsername";
      
      /// <summary>
      /// Пароль пользователя.
      /// </summary>
      public const string Password = "1СPassword";
    }
    
    /// <summary>
    /// Наименования свойств.
    /// </summary>
    public static class PropertyNames
    {
      /// <summary>
      /// Идентификатор сущности.
      /// </summary>
      public const string Ref_Key = "Ref_Key";
      
      /// <summary>
      /// Ссылка на организацию.
      /// </summary>
      public const string Организация_Key = "Организация_Key";
      
      /// <summary>
      /// Ссылка на контрагента.
      /// </summary>
      public const string Контрагент_Key = "Контрагент_Key";
      
      /// <summary>
      /// Ссылка на договор контрагента.
      /// </summary>
      public const string ДоговорКонтрагента_Key = "ДоговорКонтрагента_Key";
    }
  }
}