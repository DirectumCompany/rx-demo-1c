using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.RuleBasedApproval.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      CreateSendIncomingInvoiceTo1CStage();
    }
    
    /// <summary>
    /// Создание этапа для отправки входящего счета в 1С.
    /// </summary>
    private static void CreateSendIncomingInvoiceTo1CStage()
    {
      InitializationLogger.DebugFormat("Init: Create approval stage for sending the incoming invoice to 1C.");
      if (SendIncomingInvoiceTo1CStages.GetAll().Any())
        return;
      
      var stage = SendIncomingInvoiceTo1CStages.Create();
      stage.Name = Sungero.RuleBasedApproval.Resources.SendIncomingInvoiceTo1C;
      stage.TimeoutInHours = 4;
      stage.Save();
    }
  }
}
