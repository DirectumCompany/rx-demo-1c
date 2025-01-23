using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Sungero.NoCodeApproval.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      CreateOutgoingInvoiceExternalLinkIfNotExist();
    }
    
    /// <summary>
    /// Создать внешнюю ссылку (ExternalLink) для исходящего счета, если она не существует.
    /// </summary>
    private void CreateOutgoingInvoiceExternalLinkIfNotExist()
    {
      InitializationLogger.Debug("Init: Create OutgoingInvoice document type external link");
      var documentType = Docflow.PublicInitializationFunctions.Module.GetDocumentTypeByTypeGuid(Constants.Module.OutgoingInvoiceTypeGuid);
      if (documentType == null)
      {
        Docflow.PublicInitializationFunctions.Module.CreateDocumentType(Sungero.Contracts.Resources.OutgoingInvoiceTypeName,
                                                                        Constants.Module.OutgoingInvoiceTypeGuid,
                                                                        Docflow.DocumentType.DocumentFlow.Outgoing,
                                                                        Sungero.CoreEntities.DatabookEntry.Status.Closed,
                                                                        true);
      }
      Docflow.PublicInitializationFunctions.Module.CreateDocumentTypeExternalLink(Constants.Module.OutgoingInvoiceTypeGuid, Constants.Module.OutgoingInvoiceExternalEntityId);
    }
  }
}
