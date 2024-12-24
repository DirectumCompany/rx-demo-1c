using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;

namespace Sungero.Sync1CExample.Server.Sync1CExampleBlocks
{
  partial class CreateIIncomingInvoiceIn1CHandlers
  {
    public virtual void CreateIIncomingInvoiceIn1CExecute()
    {
      var incomingInvoice = _block.IncomingInvoice;
      var externalEntityLink = Sungero.Commons.ExternalEntityLinks.Null;
      if (incomingInvoice != null)
        externalEntityLink = Sungero.Integration1CDemo.PublicFunctions.Module.GetExternalEntityLink(incomingInvoice, null);
      else
        Logger.DebugFormat("CreateIncomingInvoiceIn1CExecute. Unable to create invoice. Document is not an incoming invoice or is null.");
      
      if (externalEntityLink == null)
      {
        var isCreated = Sungero.Integration1CDemo.PublicFunctions.Module.CreateIncomingInvoice1C(incomingInvoice);
        
        if (isCreated)
          Logger.DebugFormat("CreateIIncomingInvoiceIn1CExecute. Successfully created incoming invoice in 1C. IncomingInvoice (ID={0}).", incomingInvoice.Id);
        else
          Logger.DebugFormat("CreateIIncomingInvoiceIn1CExecute. Failed to create incoming invoice in 1C. IncomingInvoice (ID={0}).", incomingInvoice.Id);
      }
      else
        Logger.DebugFormat("CreateIIncomingInvoiceIn1CExecute. Incoming invoice already exists in 1C.");
    }
  }


  partial class SetOutgoingInvoiceStatusToPaid1CHandlers  
  {  
    public virtual void SetOutgoingInvoiceStatusToPaid1CExecute()  
    {  
      var outgoingInvoice = Contracts.OutgoingInvoices.As(_block.Document);  
      if (outgoingInvoice != null && outgoingInvoice.LifeCycleState == Contracts.OutgoingInvoice.LifeCycleState.Paid)  
      {  
        var isSuccess = Sungero.Integration1CDemo.PublicFunctions.Module.SendDocumentStatusTo1C(outgoingInvoice);  
          
        if (isSuccess)  
          Logger.DebugFormat("SetOutgoingInvoiceStatusToPaid1CExecute. Successfully updated status of the outgoing invoice to 'Paid' in 1C. OutgoingInvoice (ID={0}).", outgoingInvoice.Id);  
        else  
          Logger.DebugFormat("SetOutgoingInvoiceStatusToPaid1CExecute. Failed to update status of the outgoing invoice to 'Paid' in 1C. OutgoingInvoice (ID={0}).", outgoingInvoice.Id);  
      }  
      else  
        Logger.DebugFormat("SetOutgoingInvoiceStatusToPaid1CExecute. Unable to update status. Document is not outgoing invoice or status is not 'Paid' (ID={0}).", _block.Document.Id);  
    }  
  }

    partial class SetUniversalTransferDocumentSignStatus1CHandlers
    {
      public virtual void SetUniversalTransferDocumentSignStatus1CExecute()
      {
        var universalTransferDocument = Sungero.FinancialArchive.UniversalTransferDocuments.As(_block.Document);
        if (universalTransferDocument != null &&
            universalTransferDocument.InternalApprovalState == Sungero.FinancialArchive.UniversalTransferDocument.InternalApprovalState.Signed &&
            universalTransferDocument.ExternalApprovalState == Sungero.FinancialArchive.UniversalTransferDocument.ExternalApprovalState.Signed)
        {
          var isSuccess = Sungero.Integration1CDemo.PublicFunctions.Module.SendDocumentStatusTo1C(universalTransferDocument);
          
          if (isSuccess)
            Logger.DebugFormat("SetUniversalTransferDocumentSignStatus1CExecute. Successfully updated sign status of the document in 1C. UniversalTransferDocument (ID={0}).", universalTransferDocument.Id);
          else
            Logger.DebugFormat("SetUniversalTransferDocumentSignStatus1CExecute. Failed to update sign status of the document in 1C. UniversalTransferDocument (ID={0}).", universalTransferDocument.Id);
        }
        else
          Logger.DebugFormat("SetUniversalTransferDocumentSignStatus1CExecute. Unable to update status. Document is not universal transfer document or does not have an external or internal signature. (ID={0}).", _block.Document.Id);
        
      }
    }
  }