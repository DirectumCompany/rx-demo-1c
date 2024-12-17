using System;
using Sungero.Core;

namespace Sungero.Integration1CDemo.Constants
{
  public static class Module
  {
    /// <summary>
    /// Адрес веб-сервера 1С.
    /// </summary>
    public const string ServiceUrl1C = "https://localhost/1C_Kovalev";
    
    /// <summary>
    /// Имя пользователя 1С.
    /// </summary>
    public const string UserName1C = "admin";

    /// <summary>
    /// Пароль пользователя 1С.
    /// </summary>
    public const string Password1C = "11111";

    /// <summary>
    /// Идентификатор системы 1С.
    /// </summary>
    public const string ExtSystemId1C = "1C_ACCOUNTING";    
    
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