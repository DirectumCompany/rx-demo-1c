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
  }
}