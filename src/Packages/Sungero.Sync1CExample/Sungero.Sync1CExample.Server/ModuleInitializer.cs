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
      UpdateOData1C();
    }
    
    /// <summary>
    /// Обновить объекты OData в 1С.
    /// </summary>
    public void UpdateOData1C()
    {
      InitializationLogger.Debug("Init: Start updating list objects OData 1C.");
      var connector1C = Sungero.Integration1CExtensions.Connector1C.Get(
        Integration1CDemo.PublicFunctions.Module.GetDocflowParamsValue(Integration1CDemo.PublicConstants.Module.UserName1C),
        Integration1CDemo.PublicFunctions.Module.GetDocflowParamsValue(Integration1CDemo.PublicConstants.Module.Password1C)
       );
      
      connector1C.RunGetRequest(string.Format("{0}{1}",
                                              Integration1CDemo.PublicFunctions.Module.GetDocflowParamsValue(Integration1CDemo.PublicConstants.Module.ServiceUrl1C),
                                              Integration1CDemo.PublicConstants.Module.UpdateListObjectsODataUrlPart1C)
                               );
    }
  }
}
