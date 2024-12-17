using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.Sync1CExample.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      this.UpdateOData1C();
    }
    
    public void UpdateOData1C()
    {
      InitializationLogger.Debug("Init: Start updating list objects OData 1C.");
      var connector1C = Sungero.Integration1CExtensions.Connector1C.Get(Sungero.Integration1CDemo.PublicConstants.Module.UserName1C, Sungero.Integration1CDemo.PublicConstants.Module.Password1C);
      connector1C.RunGetRequest(string.Format("{0}{1}", Sungero.Integration1CDemo.PublicConstants.Module.ServiceUrl1C, Sungero.Integration1CDemo.PublicConstants.Module.UpdateListObjectsODataUrlPart1C));
    }
    
  }
}
