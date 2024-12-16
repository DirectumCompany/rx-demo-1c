using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;

namespace Sungero.Sync1CExample.Server.Sync1CExampleBlocks
{
  partial class SetInvoiceStatusToPaidHandlers
  {

    public virtual void SetInvoiceStatusToPaidExecute()
    {
      var attachment = _obj.Attachments.First();
      
      var outgoingInvoice = Contracts.OutgoingInvoices.As(attachment);
      if (outgoingInvoice != null && outgoingInvoice.LifeCycleState == Contracts.OutgoingInvoice.LifeCycleState.Paid)
      {
        var isSuccess = Sungero.Integration1CDemo.PublicFunctions.Module.SetInvoiceStatusToPaid(outgoingInvoice);
        
        if (isSuccess)
          Logger.DebugFormat("SetInvoiceStatusToPaidExecute. Successfully updated the status of the outgoing invoice to 'Paid' in 1C. OutgoingInvoice (ID={0}).", outgoingInvoice.Id);
        else
          Logger.DebugFormat("SetInvoiceStatusToPaidExecute. Failed to update the status of the outgoing invoice to 'Paid' in 1C. OutgoingInvoice (ID={1}).", outgoingInvoice.Id);
      }
    }
  }
}