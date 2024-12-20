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
      InsertDocflowParams();
      UpdateOData1C();
    }
    
    /// <summary>
    /// Добавить параметры в docflow_params
    /// </summary>
    public void InsertDocflowParams()
    {
      InitializationLogger.DebugFormat("Init: Adding parameters to docflow_params for integration with 1C.");
      Sungero.Docflow.PublicFunctions.Module.InsertDocflowParam(Integration1CDemo.PublicConstants.Module.ServiceUrl1C, string.Empty);
      Sungero.Docflow.PublicFunctions.Module.InsertDocflowParam(Integration1CDemo.PublicConstants.Module.UserName1C, string.Empty);
      Sungero.Docflow.PublicFunctions.Module.InsertDocflowParam(Integration1CDemo.PublicConstants.Module.Password1C, string.Empty);
      Sungero.Docflow.PublicFunctions.Module.InsertDocflowParam(Integration1CDemo.PublicConstants.Module.ExtSystemId1C, string.Empty);
    }
    
    /// <summary>
    /// Обновить объекты OData в 1С.
    /// </summary>
    /// <remarks>При первичной инициализации может возникнуть ошибка, т.к. сначала требуется заполнить новые поля из метода InsertDocflowParams.</remarks>
    public void UpdateOData1C()
    {
      try
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
      catch(Exception ex)
      {
        Logger.DebugFormat("Init: Failed access to 1C. Check that parameters in docflow_params are correct.", ex);
      }
    }
  }

}
