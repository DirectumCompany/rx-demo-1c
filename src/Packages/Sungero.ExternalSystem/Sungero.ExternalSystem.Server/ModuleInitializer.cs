using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.ExternalSystem.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      AddConnectionDocflowParams();
    }
    
    /// <summary>
    /// Добавить параметры для соединения с 1С.
    /// </summary>
    private static void AddConnectionDocflowParams()
    {
      InitializationLogger.DebugFormat("Init: Adding docflow parameters for 1C connection.");
      Sungero.Docflow.PublicFunctions.Module.InsertDocflowParam(Constants.Module.ConnectionParamNames.ServiceUrl1C, string.Empty);
      Sungero.Docflow.PublicFunctions.Module.InsertDocflowParam(Constants.Module.ConnectionParamNames.SystemId, string.Empty);
      Sungero.Docflow.PublicFunctions.Module.InsertDocflowParam(Constants.Module.ConnectionParamNames.Login, "СувороваЕА");
      Sungero.Docflow.PublicFunctions.Module.InsertDocflowParam(Constants.Module.ConnectionParamNames.Password, string.Empty);
    }
  }
}
