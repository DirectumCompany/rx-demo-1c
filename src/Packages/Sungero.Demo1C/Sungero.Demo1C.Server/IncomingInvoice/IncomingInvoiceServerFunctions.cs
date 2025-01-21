using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Demo1C.IncomingInvoice;

namespace Sungero.Demo1C.Server
{
  partial class IncomingInvoiceFunctions
  {
    /// <summary>
    /// Преобразовать в структуру данных для 1С.
    /// </summary>
    /// <returns>Структура данных 1С.</returns>
    [Public]
    public Sungero.ExternalSystem.Structures.Module.ISupplierInvoiceDto ConvertTo1cDto()
    {
      var result = Sungero.ExternalSystem.Structures.Module.SupplierInvoiceDto.Create();
      
      result.НомерВходящегоДокумента = _obj.Number.Trim();
      result.ДатаВходящегоДокумента = _obj.Date.Value;
      result.Комментарий = _obj.Note;
      result.rx_ID = _obj.Id;
      
      result.Организация_Key = Sungero.ExternalSystem.PublicFunctions.Module.GetBusinessUnit(_obj.BusinessUnit?.TIN, _obj.BusinessUnit?.TRRC);
      result.Контрагент_Key = Sungero.Demo1C.PublicFunctions.ExternalEntityLink.Remote.GetForEntityIn1C(_obj.Counterparty)?.ExtEntityId;
      result.ДоговорКонтрагента_Key = Sungero.Demo1C.PublicFunctions.ExternalEntityLink.Remote.GetForEntityIn1C(_obj.Contract)?.ExtEntityId;
      
      return result;
    }
    
    /// <summary>
    /// Заполнить недостающие данные для отправки статуса в 1С.
    /// </summary>
    /// <param name="status">Информация о статусе.</param>
    [Public]
    public static void CompleteStatusInfo(Sungero.ExternalSystem.Structures.Module.IDocumentStatusDto status)
    {
      status.Документ_Type = "StandardODATA.Document_СчетНаОплатуПоставщика";
      status.Статус = "Оплачен";
      status.Статус_Type = "UnavailableEnums.СтатусОплатыСчета";
    }
    
    /// <summary>
    /// Подготовить список услуг для передачи в 1С.
    /// </summary>
    /// <returns>Список услуг в совместимом с 1С формате.</returns>
    /// <remarks>В 1С товары и услуги именуются как "Товары".</remarks>
    [Public]
    public Sungero.ExternalSystem.Structures.Module.IServiceLineDto[] PrepareServicesForSendTo1C()
    {
      var xmlDocument = Sungero.Docflow.PublicFunctions.Module.GetNullableXmlDocument(_obj.LastVersion.Body.Read());
      return GetServicesFromXml(xmlDocument).ToArray();
    }
    
    #region private
    
    /// <summary>
    /// Получить услуги из xml-документа.
    /// </summary>
    /// <returns>Список услуг в совместимом с 1С формате.</returns>
    private static System.Collections.Generic.IEnumerable<Sungero.ExternalSystem.Structures.Module.IServiceLineDto> GetServicesFromXml(System.Xml.Linq.XDocument xmlDocument)
    {
      var servicesFromXml = xmlDocument.Element("Файл").Element("Документ").Element("ТаблСНО").Elements("СведТов");
 
      var lineNumber = 1;
      foreach (var service in servicesFromXml)
      {
        yield return new Sungero.ExternalSystem.Structures.Module.ServiceLineDto
        {
          LineNumber = lineNumber.ToString(),
          Содержание = service.Attribute("НаимТов").Value,
          Количество = service.Attribute("КолТов").Value,
          Цена = service.Attribute("ЦенаТов").Value,
          Сумма = service.Attribute("СтТовБезНДС").Value,
          СтавкаНДС = ConvertVatRateFor1C(service.Attribute("НалСт").Value),
          СуммаНДС = GetVatSumOrNull(service)
        };
        lineNumber++;
      }
    }
    
    #region Операции с НДС
    
    /// <summary>
    /// Преобразовать формат ставки НДС для передачи в 1С.
    /// </summary>
    /// <param name="vatRate">Ставка НДС.</param>
    /// <returns>Преобразованная ставка НДС.</returns>
    private static string ConvertVatRateFor1C(string vatRate)
    {
      if (vatRate == "без НДС")
        return "БезНДС";
      
      return "НДС" + vatRate.Replace("%", "").Replace("/", "_");
    }

    /// <summary>
    /// Получить сумму НДС.
    /// </summary>
    /// <param name="service">Xml-представление услуги.</param>
    /// <returns>Если у услуги есть НДС, указана сумма; иначе — null.</returns>
    private static string GetVatSumOrNull(System.Xml.Linq.XElement service)
    {
      return service.Attribute("НалСт").Value == "без НДС" || service.Attribute("НалСт").Value == "0%"
        ? null
        : service.Element("СумНал").Value;
    }
    
    #endregion
    
    #endregion
    
  }
}