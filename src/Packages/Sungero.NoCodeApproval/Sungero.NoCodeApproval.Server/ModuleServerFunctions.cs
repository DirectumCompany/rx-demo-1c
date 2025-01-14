using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.NoCodeApproval.Server
{
  public class ModuleFunctions
  {
    #region Отправка статуса в 1С
    
    /// <summary>
    /// Установить статус документа в 1C.
    /// </summary>
    /// <param name="document">Документ.</param>
    public static void SendDocumentStatusTo1C(Sungero.Docflow.IOfficialDocument document)
    {
      var externalEntityLink = Sungero.Demo1C.PublicFunctions.ExternalEntityLink.Remote.GetForEntityIn1C(document);
      if (externalEntityLink == null)
      {
        Logger.DebugFormat("NoCodeApproval.SendDocumentStatusTo1C. The document status is not updated because the entity is not synchronized with 1C. Id = {0}.", document.Id);
        return;
      }
      
      try
      {
        var dto = CreateDocumentStatusDto(document, externalEntityLink);
        
        if (Sungero.Demo1C.UniversalTransferDocuments.Is(document))
        {
          Sungero.Demo1C.PublicFunctions.UniversalTransferDocument.CompleteStatusInfo(dto);
          Sungero.ExternalSystem.PublicFunctions.Module.UpdateDocumentStatus(dto);
        }
        else if (Sungero.Demo1C.IncomingInvoices.Is(document) || Sungero.Demo1C.OutgoingInvoices.Is(document))
        {
          if (Sungero.Demo1C.IncomingInvoices.Is(document))
            Sungero.Demo1C.PublicFunctions.IncomingInvoice.CompleteStatusInfo(dto);
          else
            Sungero.Demo1C.PublicFunctions.OutgoingInvoice.CompleteStatusInfo(dto);
          
          Sungero.ExternalSystem.PublicFunctions.Module.CreateDocumentStatus(dto);
        }
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("NoCodeApproval.SendDocumentStatusTo1C. Error occurred while creating document status. DocumentId = {0}.", ex, document.Id);
      }
    }
    
    /// <summary>
    /// Сформировать структуру данных "Статусы обмена" для 1С.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="externalEntityLink">Связь сущности с внешней системой.</param>
    /// <returns>Структура данных 1С.</returns>
    private static Sungero.ExternalSystem.Structures.Module.IDocumentStatusDto CreateDocumentStatusDto(Sungero.Docflow.IOfficialDocument document,
                                                                                                       Sungero.Demo1C.IExternalEntityLink externalEntityLink)
    {
      var result = Sungero.ExternalSystem.Structures.Module.DocumentStatusDto.Create();
      result.Организация_Key = Sungero.ExternalSystem.PublicFunctions.Module.GetBusinessUnit(document.BusinessUnit?.TIN, document.BusinessUnit?.TRRC);
      result.Документ = externalEntityLink.ExtEntityId;
      return result;
    }
    
    #endregion
  }
}