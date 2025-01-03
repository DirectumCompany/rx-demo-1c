using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Demo1C.OutgoingInvoice;

namespace Sungero.Demo1C.Server
{
  partial class OutgoingInvoiceFunctions
  {
    /// <summary>
    /// Заполнить недостающие данные для отправки статуса в 1С.
    /// </summary>
    /// <param name="status">Инфоормация о статусе.</param>
    [Public]
    public static void CompleteStatusInfo(Sungero.ExternalSystem.Structures.Module.IDocumentStatusDto status)
    {
      status.Документ_Type = "StandardODATA.Document_СчетНаОплатуПокупателю";
      status.Статус = "Оплачен";
      status.Статус_Type = "UnavailableEnums.СтатусОплатыСчета";
    }
  }
}