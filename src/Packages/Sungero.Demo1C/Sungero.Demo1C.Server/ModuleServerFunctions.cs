using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Demo1C.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Сгенерировать pdf тело для формализованного документа.
    /// </summary>
    /// <param name="documentId">Идентификатор документа.</param>
    [Public(WebApiRequestType = RequestType.Post)]
    public void GeneratePublicBodyForFormalizedDocument(long documentId)
    {
      var document = Sungero.Docflow.AccountingDocumentBases.Get(documentId);
      Sungero.Docflow.PublicFunctions.Module.Remote.GeneratePublicBodyForFormalizedDocument(document, document.LastVersion.Id, null, null);
    }
    
    #region Отправить входящий счет в 1С.
    
    /// <summary>
    /// Отправить входящий счет в 1С.
    /// </summary>
    /// <param name="invoice">Входящий счет из Directum RX.</param>
    /// <returns>True - входящий счет успешно отправлен в 1С, иначе - False.</returns>
    [Public]
    public static bool SendIncomingInvoiceTo1C(Sungero.Demo1C.IIncomingInvoice invoice)
    {
      try
      {
        var invoiceDto = Sungero.Demo1C.PublicFunctions.IncomingInvoice.ConvertTo1cDto(invoice);
        var createdDtoKey = Sungero.ExternalSystem.PublicFunctions.Module.CreateIncomingInvoice(invoiceDto);
        
        if (createdDtoKey == null)
          return false;
        
        if (invoice.PaymentDueDate.HasValue)
          Sungero.ExternalSystem.PublicFunctions.Module.CreatePaymentTermForInvoice(createdDtoKey, invoiceDto.Организация_Key, invoice.PaymentDueDate.Value);
        
        Sungero.Demo1C.PublicFunctions.ExternalEntityLink.CreateNew(invoice, createdDtoKey, "СчетНаОплатуПоставщика");
        
        return true;
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Demo1C.SendIncomingInvoiceTo1C. Error while sending incoming invoice to 1C. Id = {0}.", ex, invoice.Id);
        return false;
      }
    }
    
    #endregion
  }
}