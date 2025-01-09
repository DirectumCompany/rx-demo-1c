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
      UpdateOdataObjectsIn1C();
    }
    
    /// <summary>
    /// Добавить параметры для соединения с 1С.
    /// </summary>
    private static void AddConnectionDocflowParams()
    {
      InitializationLogger.DebugFormat("Init: Adding docflow parameters for 1C connection.");
      Sungero.Docflow.PublicFunctions.Module.InsertDocflowParam(Constants.Module.ConnectionParamNames.ServiceUrl1C, string.Empty);
      Sungero.Docflow.PublicFunctions.Module.InsertDocflowParam(Constants.Module.ConnectionParamNames.SystemId, string.Empty);
    }
    
    /// <summary>
    /// Установить cостав стандартного интерфейса odata в 1С.
    /// </summary>
    private static void UpdateOdataObjectsIn1C()
    {
      try
      {
        InitializationLogger.Debug("Init: Start updating odata objects in 1C.");        
        var request = DirectumRXDemo1C.Extensions.Http.Request.Create(DirectumRXDemo1C.Extensions.Http.RequestMethod.Get, BuildUrl());
        request.Invoke();                                 
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Init: Errors occured while updating odata objects in 1C.", ex);
      }
    }
    
    /// <summary>
    /// Собрать  Url для регистрации объектов odata в 1С.
    /// </summary>
    /// <returns>Url.</returns>
    private static string BuildUrl()
    {
      var serviceUrl = Sungero.Docflow.PublicFunctions.Module.GetDocflowParamsValue(Constants.Module.ConnectionParamNames.ServiceUrl1C).ToString();
      if (string.IsNullOrEmpty(serviceUrl))
        serviceUrl = Constants.Module.DefaultServiceUrl1C;      
      
      return string.Format("{0}/hs/handlers/UpdateListObjectsOData", serviceUrl);
    }
  }
}
