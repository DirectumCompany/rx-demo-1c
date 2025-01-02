using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;

namespace Sungero.NoCodeApproval.Server.NoCodeApprovalBlocks
{
  partial class SendIncomingInvoiceTo1CHandlers
  {

    public virtual void SendIncomingInvoiceTo1CExecute()
    {
      Sungero.NoCodeApproval.Functions.Module.SendDocumentStatusTo1C(_block.Document);
    }
  }

  partial class SendPropertySignedTo1CForUniversalTransferDocumentHandlers
  {

    public virtual void SendPropertySignedTo1CForUniversalTransferDocumentExecute()
    {
      Sungero.NoCodeApproval.Functions.Module.SendDocumentStatusTo1C(_block.Document);
    }
  }

  partial class SendStatusPaidTo1CForIncomingInvoiceHandlers
  {

    public virtual void SendStatusPaidTo1CForIncomingInvoiceExecute()
    {
      Sungero.NoCodeApproval.Functions.Module.SendDocumentStatusTo1C(_block.Document);
    }
  }

  partial class SendStatusPaidTo1CForOutgoingInvoiceHandlers
  {

    public virtual void SendStatusPaidTo1CForOutgoingInvoiceExecute()
    {
      Sungero.NoCodeApproval.Functions.Module.SendDocumentStatusTo1C(_block.Document);
    }
  }
}