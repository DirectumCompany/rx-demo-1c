using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
      var url = BuildUrl("Catalog_Организации", $"$filter=ИНН eq '{tin}' and КПП eq '{trrc}'");
      var request = CreateRequest(RequestMethod.Get, url);
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
        Logger.DebugFormat("ExternalSystem.GetBusinessUnit. There are found {3} business units in 1C by TIN and TRRC. BusinessUnit.TIN = {0}, BusinessUnit.TRRC = {1}.", tin, trrc, businessUnitsCount);
        return null;
      }

      return businessUnits.FirstOrDefault()?[Sungero.ExternalSystem.Constants.Module.PropertyNames.Ref_Key].Value<string>();
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
      var request = CreateRequest(RequestMethod.Get, url);
      request.Invoke();
      
      return request.ResponseContent;
    }
    
    #endregion
    
    #region Сохранение данных
    
    #region Счета от поставщиков
    
    /// <summary>
    /// Создать счет от поставщика в 1С.
    /// </summary>
    /// <param name="dto">Структура с данными для документа.</param>
    /// <returns>ИД созданного документа.</returns>
    [Public]
    public static string CreateSupplierInvoice(Sungero.ExternalSystem.Structures.Module.ISupplierInvoiceDto dto)
    {
      var methodName = "CreateSupplierInvoice";
      if (!IsRequiredPropertiesAssigned(dto, methodName,
                                        Sungero.ExternalSystem.Constants.Module.PropertyNames.Организация_Key,
                                        Sungero.ExternalSystem.Constants.Module.PropertyNames.Контрагент_Key))
        return null;
      
      var url = BuildUrl("Document_СчетНаОплатуПоставщика");
      var request = CreateRequest(RequestMethod.Post, url);
      request.Invoke(dto);
      
      return ExtractRefKeyFromResponse(request.ResponseContent);
    }
    
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
      
      var url = BuildUrl("InformationRegister_СрокиОплатыДокументов");
      var request = CreateRequest(RequestMethod.Post, url);
      request.Invoke(dto);
    }
    
    /// <summary>
    /// Создать услуги для счета от поставщика.
    /// </summary>
    /// <param name="key">Ид счета.</param>
    /// <param name="servicesFor1C">Список услуг для передачи в 1С.</param>
    [Public]
    public static void CreateServicesForInvoice(string key, Sungero.ExternalSystem.Structures.Module.IServiceLineDto[] servicesFor1C)
    {
      var servicesCollection = new Dictionary<string, object>
      {
        { "Товары", servicesFor1C } 
      };
      var url = BuildUrl($"Document_СчетНаОплатуПоставщика(guid'{key}')");
      var request = CreateRequest(RequestMethod.Patch, url);
      request.Invoke(servicesCollection);
    }
 
    #endregion
    
    #region Поступления
    
    /// <summary>
    /// Создать поступление в 1С.
    /// </summary>
    /// <param name="dto">Структура с данными для документа.</param>
    /// <returns>ИД созданного документа.</returns>
    [Public]
    public static string CreateReceipt(Sungero.ExternalSystem.Structures.Module.IReceiptDto dto)
    {
      var methodName = "CreateReceipt";
      if (!IsRequiredPropertiesAssigned(dto, methodName,
                                        Sungero.ExternalSystem.Constants.Module.PropertyNames.Организация_Key,
                                        Sungero.ExternalSystem.Constants.Module.PropertyNames.Контрагент_Key,
                                        Sungero.ExternalSystem.Constants.Module.PropertyNames.ДоговорКонтрагента_Key))
        return null;
      
      var url = BuildUrl("Document_ПоступлениеТоваровУслуг");
      var request = CreateRequest(RequestMethod.Post, url);
      request.Invoke(dto);
      
      return ExtractRefKeyFromResponse(request.ResponseContent);
    }
    
    #endregion
    
    #region Операции со статусами документов
    
    /// <summary>
    /// Создать запись в регистре сведений "Статусы документов".
    /// </summary>
    /// <param name="dto">Структура с данными для записи.</param>
    [Public]
    public static void CreateDocumentStatus(Sungero.ExternalSystem.Structures.Module.IDocumentStatusDto dto)
    {
      var methodName = "CreateDocumentStatus";
      if (!IsRequiredPropertiesAssigned(dto, methodName,
                                        Sungero.ExternalSystem.Constants.Module.PropertyNames.Организация_Key))
        return;
      
      var url = BuildUrl("InformationRegister_СтатусыДокументов");
      var request = CreateRequest(RequestMethod.Post, url);
      
      request.Invoke(dto);
    }

    /// <summary>
    /// Обновить запись в регистре сведений "Статусы документов".
    /// </summary>
    /// <param name="dto">Структура с данными для записи.</param>
    [Public]
    public static void UpdateDocumentStatus(Sungero.ExternalSystem.Structures.Module.IDocumentStatusDto dto)
    {
      var methodName = "UpdateDocumentStatus";
      if (!IsRequiredPropertiesAssigned(dto, methodName,
                                        Sungero.ExternalSystem.Constants.Module.PropertyNames.Организация_Key))
        return;
      
      var entityKey = string.Format("(Организация_Key=guid'{0}', Документ='{1}', Документ_Type='{2}')", dto.Организация_Key, dto.Документ, dto.Документ_Type);
      var resourcePath = string.Format("InformationRegister_СтатусыДокументов{0}", entityKey);
      var url = BuildUrl(resourcePath);

      var request = CreateRequest(RequestMethod.Patch, url);
      request.Invoke(dto);
    }
    
    #endregion
    
    #region Вспомогательные методы
    
    /// <summary>
    /// Извлечь ИД сущности из ответа.
    /// </summary>
    /// <param name="response">Ответ.</param>
    /// <returns>ИД сущности в 1С.</returns>
    private static string ExtractRefKeyFromResponse(string response)
    {
      return ((JObject)JsonConvert.DeserializeObject(response))[Sungero.ExternalSystem.Constants.Module.PropertyNames.Ref_Key].ToString();
    }
    
    /// <summary>
    /// Проверить, все ли обязательные ссылки на сущности указаны.
    /// </summary>
    /// <param name="dto">Структура с данными.</param>
    /// <param name="methodName">Вызывающий метод.</param>
    /// <param name="propertyNames">Список обязательных свойств.</param>
    /// <returns>True - все обязательные свойства имеют значения.</returns>
    private static bool IsRequiredPropertiesAssigned(object dto, string methodName, params string[] propertyNames)
    {
      var entityIdForLog = GetEntityIdForLog(dto);
      
      foreach (var propertyName in propertyNames)
      {
        var propertyValue = dto.GetType().GetProperty(propertyName).GetValue(dto);
        if (propertyValue == null)
        {
          Logger.DebugFormat("ExternalSystem.{0}. The entity is not created/updated in 1C because {1} is not assigned. {2}.", methodName, propertyName, entityIdForLog);
          return false;
        }
      }
      
      return true;
    }
    
    /// <summary>
    /// Сформировать часть сообщения с идентификатором сущности для журнала.
    /// </summary>
    /// <param name="dto">Структура данных.</param>
    /// <returns>Сообщение.</returns>
    private static string GetEntityIdForLog(object dto)
    {
      var propertyRxId = dto.GetType().GetProperty("rx_ID");
      var property1cId = dto.GetType().GetProperty("Документ");
      
      return propertyRxId == null
        ? string.Format("1С Ref_Key = {0}", property1cId.GetValue(dto))
        : string.Format("RX DocumentId = {0}", propertyRxId.GetValue(dto));
    }
    
    #endregion
    
    #endregion
    
    #region Формирование запроса
    
    /// <summary>
    /// Создать запрос в 1С.
    /// </summary>
    /// <param name="method">Метод.</param>
    /// <param name="url">Url.</param>
    /// <returns>Запрос.</returns>
    public static DirectumRXDemo1C.Extensions.Http.Request CreateRequest(DirectumRXDemo1C.Extensions.Http.RequestMethod method, string url)
    {
      var result = Request.Create(method, url);
      result.UseBasicAuth(Sungero.Docflow.PublicFunctions.Module.GetDocflowParamsValue(Constants.Module.ConnectionParamNames.Login).ToString(),
                          Sungero.Docflow.PublicFunctions.Module.GetDocflowParamsValue(Constants.Module.ConnectionParamNames.Password).ToString());
      
      return result;
    }
    
    #region Формирование URL
    
    /// <summary>
    /// Собрать URL.
    /// </summary>
    /// <param name="resourcePath">Ресурс.</param>
    /// <param name="query">Параметры odata запроса.</param>
    /// <returns>Url.</returns>
    private static string BuildUrl(string resourcePath, string query = null)
    {
      var beforeFormat = query != null ? "&" : string.Empty;
      return string.Format("{0}/odata/standard.odata/{1}?{2}{3}$format=json",
                           GetBaseAddress(), resourcePath, query, beforeFormat);
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
    
    #endregion
  }
}