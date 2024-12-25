using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.SmartProcessing.Structures.Module;
using Sungero.Commons;
using Sungero.Company;
using Sungero.Docflow;
using Sungero.Integration1CExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sungero.Integration1CDemo.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Создать входящее письмо (пример использования доп. классификатора).
    /// </summary>
    /// <param name="documentInfo">Информация о документе.</param>
    /// <param name="responsible">Ответственный за верификацию.</param>
    /// <returns>Входящее письмо.</returns>
    [Public]
    public virtual IOfficialDocument CreateIncomingLetter(IDocumentInfo documentInfo,
                                                          IEmployee responsible)
    {
      // Входящее письмо.
      var document = RecordManagement.IncomingLetters.Create();
      Sungero.SmartProcessing.PublicFunctions.Module.FillIncomingLetterProperties(document, documentInfo, responsible);
      
      // Доп. классификатор.
      var additionalClassifiers = documentInfo.ArioDocument.RecognitionInfo.AdditionalClassifiers;
      if (additionalClassifiers.Count > 0)
        document.Note = string.Format("Доп. класс = {0}", additionalClassifiers.FirstOrDefault().PredictedClass);
      
      return document;
    }
    
    #region Интеграция с 1С
    
    /// <summary>
    /// Получить ссылку на связанную запись 1С.
    /// </summary>
    /// <param name="entity">Запись Directum RX.</param>
    /// <param name="extEntityType">Тип объекта 1С.</param>
    /// <returns>Структура: Hyperlink - ссылка на связанную запись 1С, ErrorMessage - текст ошибки.</returns>
    [Remote, Public]
    public virtual Structures.Module.IGetHyperlink1CResult GetSyncEntity1CHyperlink(Sungero.Domain.Shared.IEntity entity, string extEntityType)
    {
      var result = Integration1CDemo.Structures.Module.GetHyperlink1CResult.Create();
      var hyperlink = string.Empty;
      var errorMessage = string.Empty;
      
      var entityExternalLink = this.GetExternalEntityLink(entity, extEntityType);

      if (entityExternalLink == null)
      {
        errorMessage = Integration1CDemo.Resources.OpenRecord1CErrorNotExist;
      }
      else
      {
        if (entityExternalLink.IsDeleted == true)
          errorMessage =  Integration1CDemo.Resources.OpenRecord1CErrorIsDelete;
        
        try
        {
          var connector1C = this.GetConnector1C();
          hyperlink = connector1C.RunGetRequest(string.Format("{0}/hs/gethyperlink/GetHyperlink/{1}/{2}",
                                                              GetDocflowParamsValue(Constants.Module.ServiceUrl1C), entityExternalLink.ExtEntityId, entityExternalLink.ExtEntityType));
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("Integration1C. Error while getting sync entity 1C hyperlink. EntityId = {0}, ExtEntityType = {1}, ExtEntityId = {2}.", ex,
                             entity.Id, entityExternalLink.ExtEntityType, entityExternalLink.ExtEntityId);
          errorMessage = Integration1CDemo.Resources.OpenRecord1CError;
        }
      }
      
      result.Hyperlink = hyperlink;
      result.ErrorMessage = errorMessage;
      return result;
    }
    
    /// <summary>
    /// Получить ссылку на входящий счет 1С.
    /// </summary>
    /// <param name="incommingInvoice">Входящий счет.</param>
    /// <returns>Структура: Hyperlink - ссылка на входящий счет 1С, ErrorMessage - текст ошибки.</returns>
    [Remote, Public]
    public virtual Structures.Module.IGetHyperlink1CResult GetIncomingInvoice1CHyperlink(Sungero.Integration1CDemo.IIncomingInvoice incommingInvoice)
    {
      var result = Integration1CDemo.Structures.Module.GetHyperlink1CResult.Create();
      var hyperlink = string.Empty;
      var errorMessage = string.Empty;
      
      try
      {
        var connector1C = this.GetConnector1C();
        // Ограничение: работает только, если у нашей организации и контрагента заполнены поля: ИНН и КПП.
        var getHyperlinkRequestUrl = string.Format("{0}/hs/gethyperlink/GetIncomingInvoiceHyperlink/{1}/{2}/{3}/{4}/{5}/{6}",
                                                   GetDocflowParamsValue(Constants.Module.ServiceUrl1C), incommingInvoice.Number.Trim(), incommingInvoice.Date.Value.ToString("yyyy-MM-dd"),
                                                   incommingInvoice.BusinessUnit?.TIN, incommingInvoice.BusinessUnit?.TRRC,
                                                   incommingInvoice.Counterparty?.TIN, Sungero.Parties.CompanyBases.As(incommingInvoice.Counterparty)?.TRRC);
        hyperlink = connector1C.RunGetRequest(getHyperlinkRequestUrl);
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Integration1C. Error while getting incoming invoice 1C hyperlink. IncomingInvoice Id = {0}.", ex, incommingInvoice.Id);
        errorMessage = Integration1CDemo.Resources.OpenRecord1CError;
      }
      
      result.Hyperlink = hyperlink;
      result.ErrorMessage = errorMessage;
      return result;
    }
    
    /// <summary>
    /// Создать входящий счет в 1С.
    /// </summary>
    /// <param name="incommingInvoice">Входящий счет в Directum RX.</param>
    /// <returns>True - входящий счет успешно создан в 1С, иначе - False.</returns>
    [Public]
    public virtual bool CreateIncomingInvoice1C(Sungero.Integration1CDemo.IIncomingInvoice incommingInvoice)
    {
      var created = false;
      
      // Получить ссылку на контрагента.
      var counterpartyExtEntityLink = this.GetExternalEntityLink(incommingInvoice.Counterparty, Constants.Module.CounterpartyExtEntityType);
      if (counterpartyExtEntityLink == null)
      {
        Logger.DebugFormat("Integration1C. Incoming invoice not created in 1C: counterparty is not sync to 1C. IncomingInvoice Id = {0}.", incommingInvoice.Id);
        return false;
      }
      
      // Получить ИД договора в 1С.
      var contractExtEntityId = string.Empty;
      if (incommingInvoice.Contract != null)
      {
        var contractExtEntityLink = this.GetExternalEntityLink(incommingInvoice.Contract, Constants.Module.ContractsExtEntityType);
        if (contractExtEntityLink != null)
          contractExtEntityId = contractExtEntityLink.ExtEntityId;
      }
      
      try
      {
        var connector1C = this.GetConnector1C();
        
        // Получить ИД организации в 1С.
        var businessUnit1CId = this.GetBusinessUnit1CId(connector1C, incommingInvoice.BusinessUnit?.TIN, incommingInvoice.BusinessUnit?.TRRC);
        if (string.IsNullOrEmpty(businessUnit1CId))
        {
          Logger.DebugFormat("Integration1C. Incoming invoice not created in 1C: not found single business unit in 1C. IncomingInvoice Id = {0}.", incommingInvoice.Id);
          return false;
        }
        
        // Создать входящий счет в 1С.
        var incomingInvoice1C = Structures.Module.IncomingInvoice1C.Create();
        incomingInvoice1C.Организация_Key = businessUnit1CId;
        incomingInvoice1C.Контрагент_Key = counterpartyExtEntityLink.ExtEntityId;
        incomingInvoice1C.НомерВходящегоДокумента = incommingInvoice.Number.Trim();
        incomingInvoice1C.ДатаВходящегоДокумента = incommingInvoice.Date.Value;
        incomingInvoice1C.Комментарий = incommingInvoice.Note;
        incomingInvoice1C.rx_ID = incommingInvoice.Id;
        if (!string.IsNullOrEmpty(contractExtEntityId))
          incomingInvoice1C.ДоговорКонтрагента_Key = contractExtEntityId;
        
        // Примечание: для возможности работы с входящими счетами через API веб-сервера 1С необходимо выполнить один раз GET-запрос: "<Адрес веб-сервера 1С>/hs/handlers/UpdateListObjectsOData".
        var response = connector1C.RunPostRequest(string.Format("{0}{1}", GetDocflowParamsValue(Constants.Module.ServiceUrl1C), Constants.Module.CreatingIncInvoiceUrlPart1C), incomingInvoice1C);
        
        // Результат выполнения запроса (response) можно парсить либо через десериализацию в структуру либо через JObject.
        var createdIncomingInvoice1C = JsonConvert.DeserializeObject<Sungero.Integration1CDemo.Structures.Module.IncomingInvoice1C>(response);
        var createdIncomingInvoice1CId = createdIncomingInvoice1C?.Ref_Key;
        
        // Создать запись в регистре сведений "Сроки оплаты документов" в 1С.
        if (incommingInvoice.PaymentDueDate.HasValue)
        {
          var paymentTermContent = new 
          {
            Организация_Key = businessUnit1CId,
            Документ = createdIncomingInvoice1CId,
            Документ_Type = "StandardODATA.Document_СчетНаОплатуПоставщика",
            СрокОплаты = incommingInvoice.PaymentDueDate
          };
          
          var paymentTerm = connector1C.RunPostRequest(string.Format("{0}{1}", GetDocflowParamsValue(Constants.Module.ServiceUrl1C), Constants.Module.CreatingPaymentTermUrlPart1C), paymentTermContent);
        }
        
        created = !string.IsNullOrEmpty(createdIncomingInvoice1CId);
        if (created)
          this.Create1СExternalEntityLink(incommingInvoice, createdIncomingInvoice1CId, "СчетНаОплатуПоставщика");
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Integration1C. Error while getting incoming invoice 1C hyperlink. IncomingInvoice Id = {0}.", ex, incommingInvoice.Id);
        created = false;
      }
      
      return created;
    }
    
    /// <summary>
    /// Создать запись со связной сущностью в 1С.
    /// </summary>
    /// <param name="entity">Сущность.</param>
    /// <param name="externalEntityId">Идентификатор сущности в 1С.</param>
    /// <param name="externalEntityType">Тип сущности в 1С.</param>
    private void Create1СExternalEntityLink(Sungero.Domain.Shared.IEntity entity, string externalEntityId, string externalEntityType)
    {
      var externalEntityLink = ExternalEntityLinks.Create();
      externalEntityLink.EntityId = entity.Id;
      externalEntityLink.EntityType = entity.TypeDiscriminator.ToString();
      externalEntityLink.ExtEntityId = externalEntityId;
      externalEntityLink.ExtEntityType = externalEntityType;
      externalEntityLink.ExtSystemId = GetDocflowParamsValue(Constants.Module.ExtSystemId1C);
      externalEntityLink.Save();
    }
    
    /// <summary>
    /// Получить ссылку на объект внешней системы.
    /// </summary>
    /// <param name="entity">Запись Directum RX.</param>
    /// <param name="extEntityType">Тип объекта 1С.</param>
    /// <returns>Ссылка на объект внешней системы. Если не найдена, то null.</returns>
    [Public]
    public virtual IExternalEntityLink GetExternalEntityLink(Sungero.Domain.Shared.IEntity entity, string extEntityType = null)
    {
      var typeGuid = entity.TypeDiscriminator.ToString();
      var entityExternalLinks = ExternalEntityLinks.GetAll()
        .Where(x => string.Equals(x.EntityType, typeGuid, StringComparison.OrdinalIgnoreCase) &&
               x.EntityId == entity.Id &&
               x.ExtSystemId == GetDocflowParamsValue(Constants.Module.ExtSystemId1C));
      
      if (!string.IsNullOrEmpty(extEntityType))
        entityExternalLinks.Where(x => x.ExtEntityType == extEntityType);

      return entityExternalLinks.FirstOrDefault();
    }

    /// <summary>
    /// Получить ИД организации в 1С по ИНН и КПП.
    /// </summary>
    /// <param name="connector1C">Коннектор к 1С.</param>
    /// <param name="tin">ИНН.</param>
    /// <param name="trrc">КПП.</param>
    /// <returns>ИД организации в 1С. Если организация не найдена - null.</returns>
    public virtual string GetBusinessUnit1CId(Sungero.Integration1CExtensions.Connector1C connector1C, string tin, string trrc)
    {
      var response = connector1C.RunGetRequest(string.Format("{0}{1}?$filter=ИНН eq '{2}' and КПП eq '{3}'&$format=json",
                                                             GetDocflowParamsValue(Constants.Module.ServiceUrl1C), Constants.Module.GetBusinessUnitsUrlPart1C,
                                                             tin, trrc));
      
      // Результат выполнения запроса (response) можно парсить либо через десериализацию в структуру либо через JObject.
      var jsonDataResponse = (JObject)JsonConvert.DeserializeObject(response);
      var businessUnits1C = jsonDataResponse["value"];
      var businessUnits1CCount = businessUnits1C.Count();

      if (businessUnits1CCount == 0)
      {
        Logger.DebugFormat("Integration1C. Business unit by TIN and TRRC not found in 1C. BusinessUnit.TIN = {0}, BusinessUnit.TRRC = {1}.", tin, trrc);
        return null;
      }
      
      if (businessUnits1CCount > 1)
      {
        Logger.DebugFormat("Integration1C. Found {3} business units in 1C by TIN and TRRC. BusinessUnit.TIN = {0}, BusinessUnit.TRRC = {1}.", tin, trrc, businessUnits1CCount);
        return null;
      }
      
      var businessUnit1CId = businessUnits1C.FirstOrDefault()?["Ref_Key"].Value<string>();
      return businessUnit1CId;
    }
    
    /// <summary>
    /// Получить коннектор к 1С.
    /// </summary>
    /// <returns>Коннектор к 1С.</returns>
    public virtual Sungero.Integration1CExtensions.Connector1C GetConnector1C()
    {
      return Integration1CExtensions.Connector1C.Get(GetDocflowParamsValue(Constants.Module.UserName1C), GetDocflowParamsValue(Constants.Module.Password1C));
    }

    /// <summary>
    /// Установить статус документа в 1C.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>True - успешно, False - неуспешно.</returns>
    [Public]
    public virtual bool SendDocumentStatusTo1C(Sungero.Docflow.IOfficialDocument document)
    {
      var documentExtEntityLink = this.GetExternalEntityLink(document);
      
      if (documentExtEntityLink == null)
      {
        Logger.DebugFormat("Integration1C. Document status not updated in 1C: Document is not sync to 1C. Document Id = {0}.", document.Id);
        return false;
      }
      
      try
      {
        var connector1C = this.GetConnector1C();
        var document1CId = documentExtEntityLink.ExtEntityId;
        var businessUnit1CId = this.GetBusinessUnit1CId(connector1C, document.BusinessUnit?.TIN, document.BusinessUnit?.TRRC);
        
        if (string.IsNullOrEmpty(businessUnit1CId))
        {
          Logger.DebugFormat("Integration1C. Document status not updated in 1C: not found single business unit in 1C. Document Id = {0}.", document.Id);
          return false;
        }
        
        this.SendDocumentStatusTo1C(document, connector1C, document1CId, businessUnit1CId);
        return true;
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Integration1C. Error while updating document. Document Id = {0}.", ex, document.Id);
        return false;
      }
      
    }
    
    /// <summary>
    /// Отправить статус документа в 1C.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="connector1C">Коннектор 1C.</param>
    /// <param name="document1CId">Идентификатор документа в 1C.</param>
    /// <param name="businessUnit1CId">Идентификатор организации.</param>
    private void SendDocumentStatusTo1C(Sungero.Docflow.IOfficialDocument document, Sungero.Integration1CExtensions.Connector1C connector1C, string document1CId, string businessUnit1CId)
    {
      if (Sungero.FinancialArchive.UniversalTransferDocuments.Is(document))
        this.SendUniversalTransferDocumentSignStatusTo1C(connector1C, businessUnit1CId, document1CId);
      else if (Sungero.Contracts.OutgoingInvoices.Is(document))
        this.SendOutgoingInvoiceStatusTo1C(connector1C, businessUnit1CId, document1CId);
      else
        Logger.DebugFormat("Integration1C. Couldn't send status. Unsupported document type. Document (ID={0}).", document.Id);
    }

    /// <summary>
    /// Отправить запрос на смену статуса в 1С для исходящего счета.
    /// </summary>
    /// <param name="connector1C">Коннектор к 1С.</param>
    /// <param name="businessUnit1CId">Организация.</param>
    /// <param name="invoiceId">Id исходящего счёта.</param>
    private void SendOutgoingInvoiceStatusTo1C(Sungero.Integration1CExtensions.Connector1C connector1C, string businessUnit1CId, string invoice1CId)
    {
      if (this.IsDocumentStatusExistsIn1C(connector1C, businessUnit1CId, invoice1CId, Sungero.Integration1CDemo.Resources.GetOutgoingInvoiceStatusFrom1CUrl))
      {
        var statusContent = new {
          Статус = "Оплачен",
          Статус_Type = "UnavailableEnums.СтатусОплатыСчета"
        };
        
        var url = string.Format(Sungero.Integration1CDemo.Resources.PatchOutgoingInvoiceStatusFrom1CUrl, businessUnit1CId, invoice1CId);
        
        connector1C.RunPatchRequest(string.Format("{0}{1}", GetDocflowParamsValue(Constants.Module.ServiceUrl1C), url), statusContent);
      }
      else
      {
        var statusContent = new {
          Организация_Key = businessUnit1CId,
          Документ = invoice1CId,
          Документ_Type = "StandardODATA.Document_СчетНаОплатуПокупателю",
          Статус = "Оплачен",
          Статус_Type = "UnavailableEnums.СтатусОплатыСчета"
        };
        
        connector1C.RunPostRequest(string.Format("{0}{1}", GetDocflowParamsValue(Constants.Module.ServiceUrl1C), Constants.Module.CreatingDocumentStatusUrlPart1C), statusContent);
      }
    }
    
    /// <summary>
    /// Отправить запрос на смену статуса подписания в 1С для УПД.
    /// </summary>
    /// <param name="connector1C">Коннектор к 1С.</param>
    /// <param name="businessUnit1CId">Организация.</param>
    /// <param name="utd1CId">Id УПД.</param>
    private void SendUniversalTransferDocumentSignStatusTo1C(Sungero.Integration1CExtensions.Connector1C connector1C, string businessUnit1CId, string utd1CId)
    {
      if (this.IsDocumentStatusExistsIn1C(connector1C, businessUnit1CId, utd1CId, Sungero.Integration1CDemo.Resources.GetUniversalTransferDocumentStatusFrom1CUrl))
      {
        var statusContent = new {
          Статус = "Подписан",
          Статус_Type = "UnavailableEnums.СтатусыДокументовРеализации"
        };

        var url = string.Format(Sungero.Integration1CDemo.Resources.PatchUniversalTransferDocumentSignStatusFrom1C, businessUnit1CId, utd1CId);
        
        connector1C.RunPatchRequest(string.Format("{0}{1}", GetDocflowParamsValue(Constants.Module.ServiceUrl1C), url), statusContent);
      }
      else
      {
        var statusContent = new {
          Организация_Key = businessUnit1CId,
          Документ = utd1CId,
          Документ_Type = "StandardODATA.Document_РеализацияТоваровУслуг",
          Статус = "Подписан",
          Статус_Type = "UnavailableEnums.СтатусыДокументовРеализации"
        };
        
        connector1C.RunPostRequest(string.Format("{0}{1}", GetDocflowParamsValue(Constants.Module.ServiceUrl1C), Constants.Module.CreatingDocumentStatusUrlPart1C), statusContent);
      }
    }

    /// <summary>
    /// Проверить, существует ли статус для документа в 1С.
    /// </summary>
    /// <param name="connector1C">Коннектор к 1С.</param>
    /// <param name="businessUnit1CId">Организация.</param>
    /// <param name="document1CId">Id документа.</param>
    /// <param name="urlTemplate">Шаблон URL для запроса.</param>
    /// <returns>True - существует, False - не существует.</returns>
    private bool IsDocumentStatusExistsIn1C(Sungero.Integration1CExtensions.Connector1C connector1C, string businessUnit1CId, string document1CId, string urlTemplate)
    {
      try
      {
        var url = string.Format(urlTemplate, businessUnit1CId, document1CId);
        var requestString = string.Format("{0}{1}", GetDocflowParamsValue(Constants.Module.ServiceUrl1C), url);
        
        connector1C.RunGetRequest(requestString);
        
        return true;
      }
      catch
      {
        return false;
      }
    }
    
    /// <summary>
    /// Получить значение параметра из docflow_params.
    /// </summary>
    /// <param name="key">Ключ параметра.</param>
    /// <returns>Значение параметра.</returns>
    [Public]
    public string GetDocflowParamsValue(string key)
    {
      return Sungero.Docflow.PublicFunctions.Module.GetDocflowParamsValue(key).ToString();
    }

  }
  
  #endregion
}