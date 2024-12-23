using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Sync1CExample.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Сгенерировать буличное тело для формализованного документа.
    /// </summary>
    /// <param name="documentId">Идентификатор документа.</param>
    [Public(WebApiRequestType = RequestType.Post)]
    public void GeneratePublicBodyForFormalizedDocument(long documentId)
    {
      var document = Sungero.Docflow.AccountingDocumentBases.Get(documentId);
      Sungero.Docflow.PublicFunctions.Module.Remote.GeneratePublicBodyForFormalizedDocument(document, document.LastVersion.Id, null, null);
    }
  }
}