using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.ContractsExample.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      CreateApprovalCreateIncInvoice1CStage();
      InsertDocflowParams();
    }
    
    /// <summary>
    /// Создание этапа создания входящего счета в 1С.
    /// </summary>
    public static void CreateApprovalCreateIncInvoice1CStage()
    {
      InitializationLogger.DebugFormat("Init: Create approval stage for creating incoming invoice in 1C.");
      if (ApprovalCreateIncInvoice1CStages.GetAll().Any())
        return;
      
      var stage = ApprovalCreateIncInvoice1CStages.Create();
      stage.Name = Sungero.ContractsExample.Resources.ApprovalCreateIncInvoice1CStageName;
      stage.TimeoutInHours = 4;
      stage.Save();
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
  }
}
