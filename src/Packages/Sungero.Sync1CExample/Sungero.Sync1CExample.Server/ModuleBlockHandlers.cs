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
        var isSuccess = Sungero.Integration1CDemo.PublicFunctions.Module.SetInvoiceStatusToPaid1C(outgoingInvoice);
        
        if (isSuccess)
          Logger.DebugFormat("SetInvoiceStatusToPaidExecute. Successfully updated status of the outgoing invoice to 'Paid' in 1C. OutgoingInvoice (ID={0}).", outgoingInvoice.Id);
        else
          Logger.DebugFormat("SetInvoiceStatusToPaidExecute. Failed to update status of the outgoing invoice to 'Paid' in 1C. OutgoingInvoice (ID={0}).", outgoingInvoice.Id);
      }
      else
        Logger.ErrorFormat("SetInvoiceStatusToPaidExecute. Unable to update status. Document is not outgoing invoice or status is not 'Paid' (ID={0}).", outgoingInvoice.Id);
    }
  }
}