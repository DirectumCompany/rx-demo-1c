using DirectumRXDemo1C.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
      var url = BuildUrl("Catalog_Организации", $"ИНН eq '{tin}' and КПП eq '{trrc}'");
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
        Logger.DebugFormat("ExternalSystem.CreateIncomingInvoice. The incoming invoice is not created in 1C because counterparty is not found in 1C. Id = {0}.", dto.rx_ID);
        return null;
      }
      
      if (dto.Организация_Key == null)
      {
        Logger.DebugFormat("ExternalSystem.CreateIncomingInvoice. The incoming invoice is not created in 1C because business unit is not found in 1C or more than one. Id = {0}.", dto.rx_ID);
        return null;
      }
      
      var url = BuildUrl("Document_СчетНаОплатуПоставщика", null, "*");
      var request = Request.Create(RequestMethod.Post, url);
      request.Invoke(dto);
      
      return JsonConvert.DeserializeObject<Sungero.ExternalSystem.Structures.Module.IIncomingInvoiceDto>(request.ResponseContent)?.Ref_Key;
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
      
      var url = BuildUrl("InformationRegister_СрокиОплатыДокументов", null, "*");
      var request = Request.Create(RequestMethod.Post, url);
      request.Invoke(dto);
    }
    
    #endregion
    
    #region Формирование URL
    
    /// <summary>
    /// Собрать URL для запроса.
    /// </summary>
    /// <param name="entityName">Наименование сущности</param>
    /// <param name="filterValue">Значение фильтра.</param>
    /// <param name="expandValue">Значение параметра "expand"</param>
    /// <returns></returns>
    private static string BuildUrl(string entityName, string filterValue, string expandValue = null)
    {
      var serviceUrl = Sungero.Docflow.PublicFunctions.Module.GetDocflowParamsValue(Constants.Module.ConnectionParamNames.ServiceUrl1C);
      
      var filter = filterValue != null ? string.Format("&$filter={0}", filterValue) : string.Empty;
      var expand = expandValue != null ? string.Format("&$expand={0}", expandValue) : string.Empty;
      
      return string.Format("{0}/odata/standard.odata/{1}?{2}&$format=json{3}", serviceUrl, entityName, filter, expand);
    }
    
    #endregion
  }
}