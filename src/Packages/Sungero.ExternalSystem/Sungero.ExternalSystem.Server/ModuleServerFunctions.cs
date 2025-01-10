using System;
using System.Collections.Generic;
using System.Linq;
using DirectumRXDemo1C.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.ExternalSystem.Server
{
  public class ModuleFunctions
  {
    #region Получение данных
    
    /// <summary>
    /// Получить системный ИД 1С из параметров документооборота.
    /// </summary>
    /// <returns>Системный ИД 1С.</returns>
    [Public]
    public static string GetId()
    {
      return Sungero.Docflow.PublicFunctions.Module.GetDocflowParamsValue(Constants.Module.ConnectionParamNames.SystemId).ToString();
    }
    
    /// <summary>
    /// Получить ИД организации по ИНН и КПП.
    /// </summary>
    /// <param name="tin">ИНН.</param>
    /// <param name="trrc">КПП.</param>
    /// <returns>ИД организации в 1С. Если организация не найдена - null.</returns>
    [Public]
    public static string GetBusinessUnit(string tin, string trrc)
    {
      var url = BuildGetUrl("Catalog_Организации", $"ИНН eq '{tin}' and КПП eq '{trrc}'");
      var request = Request.Create(RequestMethod.Get, url);
      request.Invoke();
      
      var jsonDataResponse = (JObject)JsonConvert.DeserializeObject(request.ResponseContent);
      var businessUnits = jsonDataResponse["value"];
      var businessUnitsCount = businessUnits.Count();

      if (businessUnitsCount == 0)
      {
        Logger.DebugFormat("ExternalSystem.GetBusinessUnit. Business unit by TIN and TRRC are not found in 1C. BusinessUnit.TIN = {0}, BusinessUnit.TRRC = {1}.", tin, trrc);
        return null;
      }

      if (businessUnitsCount > 1)
      {
        Logger.DebugFormat("ExternalSystem.GetBusinessUnit. There  are found {3} business units in 1C by TIN and TRRC. BusinessUnit.TIN = {0}, BusinessUnit.TRRC = {1}.", tin, trrc, businessUnitsCount);
        return null;
      }

      return businessUnits.FirstOrDefault()?["Ref_Key"].Value<string>();
    }
    
    /// <summary>
    /// Получить ссылку на сущноссть в 1С.
    /// </summary>
    /// <param name="entityId">ИД сущности.</param>
    /// <param name="entityType">Тип сущности.</param>
    /// <returns>Ссылка на сущность.</returns>
    [Public, Remote(IsPure = true)]
    public string GetEntityLink(string entityId, string entityType)
    {
      var url = string.Format("{0}/hs/gethyperlink/GetHyperlink/{1}/{2}", GetBaseAddress(), entityId, entityType);
      var request = Request.Create(RequestMethod.Get, url);
      request.Invoke();
      
      return request.ResponseContent;
    }
    
    #endregion
    
    #region Сохранение данных
    
    #region Создание документов
    
    /// <summary>
    /// Создать счет от поставщика в 1С.
    /// </summary>
    /// <param name="dto">Структура с данными для документа.</param>
    /// <returns>ИД созданного документа.</returns>
    [Public]
    public static string CreateSupplierInvoice(Sungero.ExternalSystem.Structures.Module.ISupplierInvoiceDto dto)
    {
      if (!IsConterpartyAndOrganizationFound(dto.Организация_Key, dto.Контрагент_Key, "SupplierInvoice", dto.rx_ID))
        return null;
      
      var url = BuildPostUrl("Document_СчетНаОплатуПоставщика");
      var request = Request.Create(RequestMethod.Post, url);
      request.Invoke(dto);
      
      return ((JObject)JsonConvert.DeserializeObject(request.ResponseContent))["Ref_Key"].ToString();
    }
    
    /// <summary>
    /// Создать поступление в 1С.
    /// </summary>
    /// <param name="dto">Структура с данными для документа.</param>
    /// <returns>ИД созданного документа.</returns>
    [Public]
    public static string CreateReceipt(Sungero.ExternalSystem.Structures.Module.IReceiptDto dto)
    {      
     if (!IsConterpartyAndOrganizationFound(dto.Организация_Key, dto.Контрагент_Key, "Receipt", dto.rx_ID))
        return null;
      
      if (dto.ДоговорКонтрагента_Key == null)
      {
        Logger.DebugFormat("ExternalSystem.CreateReceipt. The document is not created in 1C because contract is not found. Id = {0}.", dto.rx_ID);
        return null;
      }
      
      var url = BuildPostUrl("Document_ПоступлениеТоваровУслуг");
      var request = Request.Create(RequestMethod.Post, url);
      request.Invoke(dto);
      
      return ((JObject)JsonConvert.DeserializeObject(request.ResponseContent))["Ref_Key"].ToString();
    }
    
    /// <summary>
    /// Проверить найдены ли контрагент и НОР в 1С.
    /// </summary>
    /// <param name="businessUnit">НОР.</param>
    /// <param name="counterparty">Контрагент.</param>
    /// <param name="documentName">Наименование создаваемого документа.</param>
    /// <param name="id">ИД документа в RX.</param>
    /// <returns>True - найдены обе сущности.</returns>
    private static bool IsConterpartyAndOrganizationFound(string businessUnit, string counterparty, string documentName, long id)
    {
      if (counterparty == null)
      {
        Logger.DebugFormat("ExternalSystem.Create{0}. The document is not created in 1C because counterparty is not found. Id = {1}.", documentName, id);
        return false;
      }
      
      if (businessUnit == null)
      {
        Logger.DebugFormat("ExternalSystem.Create{0}. The document is not created in 1C because business unit is not found or more than one. Id = {1}.", documentName, id);
        return false;
      }
      return true;
    }
    
    #endregion
    
    /// <summary>
    /// Создать срок оплаты для входящего счета.
    /// </summary>
    /// <param name="key">ИД входящего счета.</param>
    /// <param name="businessUnitKey">ИД организации.</param>
    /// <param name="paymentDueDate">Срок оплаты.</param>
    [Public]
    public static void CreatePaymentTermForInvoice(string key, string businessUnitKey, DateTime paymentDueDate)
    {
      var dto = Sungero.ExternalSystem.Structures.Module.PaymentTermDto.Create();
      dto.Организация_Key = businessUnitKey;
      dto.Документ = key;
      dto.Документ_Type = "StandardODATA.Document_СчетНаОплатуПоставщика";
      dto.СрокОплаты = paymentDueDate;
      
      var url = BuildPostUrl("InformationRegister_СрокиОплатыДокументов");
      var request = Request.Create(RequestMethod.Post, url);
      request.Invoke(dto);
    }
    
    /// <summary>
    /// Создать запись в регистре сведений "Статусы документов".
    /// </summary>
    /// <param name="dto">Структура с данными для записи.</param>
    [Public]
    public static void CreateDocumentStatus(Sungero.ExternalSystem.Structures.Module.IDocumentStatusDto dto)
    {
      if (dto.Организация_Key == null)
      {
        Logger.DebugFormat("ExternalSystem.CreateDocumentStatus. The document status is not created in 1C because business unit is not found or more than one. Id = {0}.", dto.Документ);
        return;
      }
      
      var url = BuildPostUrl("InformationRegister_СтатусыДокументов");
      var request = Request.Create(RequestMethod.Post, url);
      request.Invoke(dto);
    }
    
    #endregion
    
    #region Формирование URL
    
    /// <summary>
    /// Собрать URL для GET запроса.
    /// </summary>
    /// <param name="entityName">Наименование сущности.</param>
    /// <param name="filterValue">Значение фильтра.</param>
    /// <returns>Url.</returns>
    private static string BuildGetUrl(string entityName, string filterValue = null)
    {
      return BuildUrl(entityName, filterValue, null);
    }
    
    /// <summary>
    /// Собрать URL для POST запроса.
    /// </summary>
    /// <param name="entityName">Наименование сущности.</param>
    /// <returns>Url.</returns>
    private static string BuildPostUrl(string entityName)
    {
      return BuildUrl(entityName, null, "*");
    }
    
    /// <summary>
    /// Собрать URL для запроса.
    /// </summary>
    /// <param name="entityName">Наименование сущности.</param>
    /// <param name="filterValue">Значение фильтра.</param>
    /// <param name="expandValue">Значение параметра "expand".</param>
    /// <returns>Url.</returns>
    private static string BuildUrl(string entityName, string filterValue, string expandValue = null)
    {
      var filter = filterValue != null ? string.Format("&$filter={0}", filterValue) : string.Empty;
      var expand = expandValue != null ? string.Format("&$expand={0}", expandValue) : string.Empty;
      
      return string.Format("{0}/odata/standard.odata/{1}?{2}&$format=json{3}", GetBaseAddress(), entityName, filter, expand);
    }
    
    /// <summary>
    /// Вернуть базовый адрес.
    /// </summary>
    /// <returns>Базовый адрес.</returns>
    private static string GetBaseAddress()
    {
      return Sungero.Docflow.PublicFunctions.Module.GetDocflowParamsValue(Constants.Module.ConnectionParamNames.ServiceUrl1C).ToString();
    }
    
    #endregion
  }
}