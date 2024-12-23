using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;

namespace Sungero.Sync1CExample.Server.Sync1CExampleBlocks
{
  partial class SetInvoiceStatusToPaid1CHandlers
  {
    public virtual void SetInvoiceStatusToPaid1CExecute()
    {
      var outgoingInvoice = Contracts.OutgoingInvoices.As(_block.Document);
      if (outgoingInvoice != null && outgoingInvoice.LifeCycleState == Contracts.OutgoingInvoice.LifeCycleState.Paid)
      {
        var isSuccess = Sungero.Integration1CDemo.PublicFunctions.Module.SendDocumentStatus(outgoingInvoice);
        
        if (isSuccess)
          Logger.DebugFormat("SetInvoiceStatusToPaidExecute. Successfully updated status of the outgoing invoice to 'Paid' in 1C. OutgoingInvoice (ID={0}).", outgoingInvoice.Id);
        else
          Logger.DebugFormat("SetInvoiceStatusToPaidExecute. Failed to update status of the outgoing invoice to 'Paid' in 1C. OutgoingInvoice (ID={0}).", outgoingInvoice.Id);
      }
      else
        Logger.DebugFormat("SetInvoiceStatusToPaidExecute. Unable to update status. Document is not outgoing invoice or status is not 'Paid' (ID={0}).", outgoingInvoice.Id);
    }
  }

  partial class SetUniversalTransferDocumentSignStatusHandlers
  {
    public virtual void SetUniversalTransferDocumentSignStatusExecute()
    {
      var universlTransferDocument = Sungero.FinancialArchive.UniversalTransferDocuments.As(_block.Document);
      if (universlTransferDocument != null &&
          universlTransferDocument.InternalApprovalState == Sungero.FinancialArchive.UniversalTransferDocument.InternalApprovalState.Signed &&
          universlTransferDocument.ExternalApprovalState == Sungero.FinancialArchive.UniversalTransferDocument.ExternalApprovalState.Signed)
      {
        var isSuccess = Sungero.Integration1CDemo.PublicFunctions.Module.SendDocumentStatus(universlTransferDocument);
        
        if (isSuccess)
          Logger.DebugFormat("SetUniversalTransferDocumentSignStatusExecute. Successfully updated sign status of the document in 1C. UniversalTransferDocument (ID={0}).", universlTransferDocument.Id);
        else
          Logger.DebugFormat("SetUniversalTransferDocumentSignStatusExecute. Failed to update sign status of the document in 1C. UniversalTransferDocument (ID={0}).", universlTransferDocument.Id);
      }
      else
        Logger.DebugFormat("SetUniversalTransferDocumentSignStatusExecute. Unable to update status. Document is not universal transfer document or does not have an external or internal signature. UniversalTransferDocument (ID={0}).", universlTransferDocument.Id);
      
    }
  }
}