using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;

namespace Sungero.NoCodeApproval.Server.NoCodeApprovalBlocks
{
  partial class SendUniversalTransferDocumentTo1CHandlers
  {

    public virtual void SendUniversalTransferDocumentTo1CExecute()
    {
      Sungero.NoCodeApproval.Functions.Module.SendUniversalTransferDocumentTo1C(_block.Document);
    }
  }

  partial class SendContractTo1CHandlers
  {

    public virtual void SendContractTo1CExecute()
    {
      Sungero.NoCodeApproval.Functions.Module.SendContractTo1C();
    }
  }

  partial class SendIncomingInvoiceTo1CHandlers
  {

    public virtual void SendIncomingInvoiceTo1CExecute()
    {
      Sungero.Demo1C.PublicFunctions.Module.SendIncomingInvoiceTo1C(_block.Document);
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