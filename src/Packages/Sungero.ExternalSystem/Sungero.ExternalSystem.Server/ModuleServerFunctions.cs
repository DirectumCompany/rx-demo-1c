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
      var url = BuildGetUrl("Catalog_Организации", $"ИНН eq '{tin}' and КПП eq '{trrc}'");
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
      var login = Sungero.Docflow.PublicFunctions.Module.GetDocflowParamsValue(Constants.Module.ConnectionParamNames.Login).ToString();
      var request = CreateRequest(RequestMethod.Get, url);
      request.Invoke();
      
      return request.ResponseContent;
    }
    
    #endregion
    
    #region Сохранение данных
    
    /// <summary>
    /// Создать входящий счет в 1С.
    /// </summary>
    /// <param name="dto">Структура с данными для документа.</param>
    /// <returns>ИД созданного документа.</returns>
    [Public]
    public static string CreateIncomingInvoice(Sungero.ExternalSystem.Structures.Module.IIncomingInvoiceDto dto)
    {
      if (dto.Контрагент_Key == null)
      {
        Logger.DebugFormat("ExternalSystem.CreateIncomingInvoice. The incoming invoice is not created in 1C because counterparty is not found. Id = {0}.", dto.rx_ID);
        return null;
      }
      
      if (dto.Организация_Key == null)
      {
        Logger.DebugFormat("ExternalSystem.CreateIncomingInvoice. The incoming invoice is not created in 1C because business unit is not found or more than one. Id = {0}.", dto.rx_ID);
        return null;
      }
      
      var url = BuildPostUrl("Document_СчетНаОплатуПоставщика");
      var request = CreateRequest(RequestMethod.Post, url);
      request.Invoke(dto);
      
      return ((JObject)JsonConvert.DeserializeObject(request.ResponseContent))["Ref_Key"].ToString();
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
      
      var url = BuildPostUrl("InformationRegister_СрокиОплатыДокументов");
      var request = CreateRequest(RequestMethod.Post, url);
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
      var request = CreateRequest(RequestMethod.Post, url);
      
      
      request.Invoke(dto);
    }
    
    #endregion
    
    #region Формирование запроса
    
    /// <summary>
    /// Создать запрос в 1С.
    /// </summary>
    /// <param name="method">Метод.</param>
    /// <param name="url">Url.</param>
    /// <returns></returns>
    public static DirectumRXDemo1C.Extensions.Http.Request CreateRequest(DirectumRXDemo1C.Extensions.Http.RequestMethod method, string url)
    {
      var login = Sungero.Docflow.PublicFunctions.Module.GetDocflowParamsValue(Constants.Module.ConnectionParamNames.Login).ToString();
      var password = Sungero.Docflow.PublicFunctions.Module.GetDocflowParamsValue(Constants.Module.ConnectionParamNames.Password).ToString();
      var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));
      
      var request =  Request.Create(method, url, login, password);
      
      1 
      var request =  Request.Create(method, url, basicAuth);      
      2 
        
      var request =  Request.Create(method, url);
      result.UseBasicAuth(login, password);      
      request.Invoke(dto);
      
      return result;
    }
    
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
    
    #endregion
  }
}