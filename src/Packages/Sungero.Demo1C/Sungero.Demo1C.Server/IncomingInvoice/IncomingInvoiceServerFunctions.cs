using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Demo1C.IncomingInvoice;
using DiadocVatRates = Sungero.Demo1C.Constants.Contracts.IncomingInvoice.VatRatesFromDiadoc;
using OneCVatRates = Sungero.Demo1C.Constants.Contracts.IncomingInvoice.VatRatesFrom1C;

namespace Sungero.Demo1C.Server
{
  partial class IncomingInvoiceFunctions
  {
    /// <summary>
    /// Преобразовать в структуру данных для 1С.
    /// </summary>
    /// <returns>Структура данных 1С.</returns>
    [Public]
    public Sungero.ExternalSystem.Structures.Module.IIncomingInvoiceDto ConvertTo1cDto()
    {
      var result = Sungero.ExternalSystem.Structures.Module.IncomingInvoiceDto.Create();
      
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
    /// <param name="invoice">Входящий счет.</param>
    /// <returns>Список услуг в совместимом с 1С формате.</returns>
    /// <remarks>В 1С товары и услуги именуются как "Товары".</remarks>
    [Public]
    public System.Collections.Generic.Dictionary<string, List<Sungero.ExternalSystem.Structures.Module.IServiceDto>> PreparingServicesForSendTo1C()
    {
      var servicesCollection = new List<Sungero.ExternalSystem.Structures.Module.IServiceDto>();
      servicesCollection.AddRange(GetServicesFromXml(_obj));
      
      if (!servicesCollection.Any())
        return null;
      
      return new Dictionary<string, List<Sungero.ExternalSystem.Structures.Module.IServiceDto>>
      {
        {"Товары", servicesCollection}
      };
    }
    
    #region private
    
    /// <summary>
    /// Получить услуги из xml-документа.
    /// </summary>
    /// <param name="invoice">Входящий счет.</param>
    /// <returns>Список услуг в совместимом с 1С формате.</returns>
    private static System.Collections.Generic.IEnumerable<Sungero.ExternalSystem.Structures.Module.IServiceDto> GetServicesFromXml(Sungero.Demo1C.IIncomingInvoice invoice)
    {
      var xmlDocument = Sungero.Docflow.PublicFunctions.Module.GetNullableXmlDocument(invoice.LastVersion.Body.Read());
      var servicesFromXml = xmlDocument.Element("Файл")?.Element("Документ")?.Element("ТаблСНО")?.Elements("СведТов");
      
      if (servicesFromXml == null)
        yield break;
      
      var lineNumber = 1;
      foreach (var service in servicesFromXml)
      {
        yield return new Sungero.ExternalSystem.Structures.Module.ServiceDto
        {
          LineNumber = lineNumber.ToString(),
          Содержание = service.Attribute("НаимТов")?.Value,
          Количество = service.Attribute("КолТов")?.Value,
          Цена = service.Attribute("ЦенаТов")?.Value,
          Сумма = service.Attribute("СтТовБезНДС")?.Value,
          СтавкаНДС = ConvertVatRateFor1C(service.Attribute("НалСт")?.Value),
          СуммаНДС = GetVatSum(service)
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
      switch (vatRate)
      {
        case DiadocVatRates.NoVat:
          return OneCVatRates.NoVat;
        case DiadocVatRates.Vat0:
          return OneCVatRates.Vat0;
        case DiadocVatRates.Vat5:
          return OneCVatRates.Vat5;
        case DiadocVatRates.Vat5_105:
          return OneCVatRates.Vat5_105;
        case DiadocVatRates.Vat7:
          return OneCVatRates.Vat7;
        case DiadocVatRates.Vat7_107:
          return OneCVatRates.Vat7_107;
        case DiadocVatRates.Vat10:
          return OneCVatRates.Vat10;
        case DiadocVatRates.Vat10_110:
          return OneCVatRates.Vat10_110;
        case DiadocVatRates.Vat18:
          return OneCVatRates.Vat18;
        case DiadocVatRates.Vat18_118:
          return OneCVatRates.Vat18_118;
        case DiadocVatRates.Vat20_120:
          return OneCVatRates.Vat20_120;
        case DiadocVatRates.Vat20:
          return OneCVatRates.Vat20;
        default:
          throw new ArgumentException($"Неизвестная ставка НДС: {vatRate}");
      }
    }

    /// <summary>
    /// Получить сумму НДС.
    /// </summary>
    /// <param name="service">Xml-представление услуги.</param>
    /// <returns>Сумма НДС.</returns>
    private static string GetVatSum(System.Xml.Linq.XElement service)
    {
      if (service.Attribute("НалСт")?.Value == DiadocVatRates.NoVat || service.Attribute("НалСт")?.Value == DiadocVatRates.Vat0)
        return null;
      
      return service.Element("СумНал")?.Value;
    }
    
    #endregion
    
    #endregion
    
  }
}