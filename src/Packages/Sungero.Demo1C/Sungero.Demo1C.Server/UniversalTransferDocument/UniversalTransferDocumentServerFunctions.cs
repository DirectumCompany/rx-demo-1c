using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Demo1C.UniversalTransferDocument;

namespace Sungero.Demo1C.Server
{
  partial class UniversalTransferDocumentFunctions
  {
    /// <summary>
    /// Заполнить недостающие данные для отправки статуса в 1С.
    /// </summary>
    /// <param name="status">Инфоормация о статусе.</param>
    [Public]
    public static void CompleteStatusInfo(Sungero.ExternalSystem.Structures.Module.IDocumentStatusDto status)
    {
      status.Документ_Type = "StandardODATA.Document_РеализацияТоваровУслуг";
      status.Статус = "Подписан";
      status.Статус_Type = "UnavailableEnums.СтатусыДокументовРеализации";
    }
  }
}