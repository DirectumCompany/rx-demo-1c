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
    #region Подготовка данных счета для 1С
    
    /// <summary>
    /// Подготовить данные счета для передачи в 1С.
    /// </summary>
    /// <returns>Структура данных 1С.</returns>
    [Public]
    public Sungero.ExternalSystem.Structures.Module.ISupplierInvoiceDto ConvertTo1cHeaderDto()
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
    /// Подготовить список услуг для передачи в 1С.
    /// </summary>
    /// <returns>Список услуг в совместимом с 1С формате.</returns>
    [Public]
    public Sungero.ExternalSystem.Structures.Module.IServiceLineDto[] ConvertTo1cServiceDtoS()
    {
      var xmlDocument = Sungero.Docflow.PublicFunctions.Module.GetNullableXmlDocument(_obj.LastVersion.Body.Read());
      return GetServicesFromXml(xmlDocument).ToArray();
    }
    
    /// <summary>
    /// Получить услуги из xml-документа в совместимом с 1С формате.
    /// </summary>
    /// <param name="xmlDocument">Xml-документ.</param>
    /// <returns>Список услуг.</returns>
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
          СуммаНДС = service.Element("СумНал")?.Value
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
      
      return "НДС" + vatRate.Replace("%", string.Empty).Replace("/", "_");
    }

    #endregion
    
    #endregion
    
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
    /// Проверить, является ли счет формализованным в системе Диадок.
    /// </summary>
    /// <returns>True - является; иначе - False</returns>
    [Public]
    public bool IsInvoiceDiadocFormalized()
    {
      var xml = Sungero.Docflow.PublicFunctions.Module.GetNullableXmlDocument(_obj.LastVersion.Body.Read());
      return Exchange.PublicFunctions.Module.GetInvoiceType(xml, null) == Sungero.Exchange.PublicConstants.Module.InvoiceTypes.DiadocFormalized;
    }
  }
}