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
    /// <returns>Список услуг для передачи в 1С.</returns>
    /// <remarks>В 1С товары и услуги именуются как "Товары".</remarks>
    [Public]
    public System.Collections.Generic.Dictionary<string, object> PreparingServicesForSendTo1C()
    {
      var servicesCollection = new List<Sungero.ExternalSystem.Structures.Module.IServiceDto>();
      servicesCollection.AddRange(GetServicesFromXml(_obj));
      return new Dictionary<string, object>
      {
        {"Товары", servicesCollection}
      };
    }
    
    /// <summary>
    /// Получить услуги из xml-документа.
    /// </summary>
    /// <param name="status">Информация о статусе.</param>
    /// <returns>Список услуг в формате для передачи в 1С.</returns>
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
          Сумма = service.Attribute("СтТовУчНал")?.Value
        };
        lineNumber++;
      }
    }
    
  }
}