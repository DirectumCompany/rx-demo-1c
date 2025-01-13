using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Demo1C.UniversalTransferDocument;

namespace Sungero.Demo1C.Client
{
  partial class UniversalTransferDocumentActions
  {
    public virtual void OpenEntityIn1CSungero(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var warning = Sungero.Demo1C.PublicFunctions.Module.OpenEntityIn1CFor(_obj);
      if (warning != null)
        e.AddWarning(warning);
    }

    public virtual bool CanOpenEntityIn1CSungero(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}