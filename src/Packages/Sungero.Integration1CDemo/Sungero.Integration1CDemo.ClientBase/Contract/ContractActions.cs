using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Integration1CDemo.Contract;

namespace Sungero.Integration1CDemo.Client
{
  partial class ContractActions
  {
    public virtual void OpenEntity1CSungero(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var getHyperlinkResult = Sungero.Integration1CDemo.PublicFunctions.Module.Remote.GetSyncEntity1CHyperlink(_obj, Constants.Module.ContractsExtEntityType);
      
      var hyperlink = getHyperlinkResult.Hyperlink;
      var errorMessage = getHyperlinkResult.ErrorMessage;
      
      if (!string.IsNullOrEmpty(hyperlink))
        Hyperlinks.Open(hyperlink);
      else if (!string.IsNullOrEmpty(errorMessage))
        e.AddWarning(errorMessage);
      else
        e.AddWarning(Integration1CDemo.Resources.OpenRecord1CError);
    }

    public virtual bool CanOpenEntity1CSungero(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

  }

}