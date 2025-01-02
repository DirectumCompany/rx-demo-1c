using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Demo1C.OutgoingInvoice;

namespace Sungero.Demo1C.Client
{
  partial class OutgoingInvoiceActions
  {
    public virtual void OpenEntityIn1CSungero(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      
    }

    public virtual bool CanOpenEntityIn1CSungero(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}