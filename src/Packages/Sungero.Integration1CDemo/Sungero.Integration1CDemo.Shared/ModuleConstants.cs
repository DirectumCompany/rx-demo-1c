using System;
using Sungero.Core;
using Sungero.Docflow;

namespace Sungero.Integration1CDemo.Constants
{
  public static class Module
  {    
    /// <summary>
    /// Адрес веб-сервера 1С.
    /// </summary>
    [Sungero.Core.Public]
    public const string ServiceUrl1C = "1CServiceAddress";
    
    /// <summary>
    /// Имя пользователя 1С.
    /// </summary>
    [Sungero.Core.Public]
    public const string UserName1C = "1СUsername";

    /// <summary>
    /// Пароль пользователя 1С.
    /// </summary>
    [Sungero.Core.Public]
    public const string Password1C = "1СPassword";

    /// <summary>
    /// Идентификатор системы 1С.
    /// </summary>
    [Sungero.Core.Public]
    public const string ExtSystemId1C = "1CSystemId";
    
    /// <summary>
    /// Тип объекта системы 1C для договоров.
    /// </summary>
    public const string ContractsExtEntityType = "ДоговорыКонтрагентов";
    
    /// <summary>
    /// Тип объекта системы 1C для контрагентов.
    /// </summary>
    public const string CounterpartyExtEntityType = "Контрагенты";
    
    /// <summary>
    /// Тип объекта системы 1C для контрагентов.
    /// </summary>
    public const string OutgoingInvoiceExtEntityType = "СчетНаОплатуПокупателю";    
    
    /// <summary>
    /// Часть пути запроса для создания входящего счета в 1С.
    /// </summary>
    public const string CreatingIncInvoiceUrlPart1C = "/odata/standard.odata/Document_СчетНаОплатуПоставщика?$format=json&$expand=*";

    /// <summary>
    /// Часть пути запроса для создания записи в регистре сведений "Сроки оплаты документов" в 1С.
    /// </summary>
    public const string CreatingPaymentTermUrlPart1C = "/odata/standard.odata/InformationRegister_СрокиОплатыДокументов?$format=json&$expand=*";

    /// <summary>
    /// Часть пути запроса для обращения к справочнику "Организации" в 1С.
    /// </summary>
    public const string GetBusinessUnitsUrlPart1C = "/odata/standard.odata/Catalog_Организации";
  }
}