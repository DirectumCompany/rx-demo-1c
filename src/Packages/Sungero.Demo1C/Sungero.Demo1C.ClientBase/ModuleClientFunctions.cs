using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Sungero.Demo1C.Client
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Открыть документ в 1С.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Текст предупреждения, если переход не состоялся.</returns>
    [Public]
    public static string OpenEntityIn1CFor(Sungero.Docflow.IOfficialDocument document)
    {
      var externalEntityLink = Sungero.Demo1C.PublicFunctions.ExternalEntityLink.Remote.GetForEntityIn1C(document);

      if (externalEntityLink == null)
        return Integration1CDemo.Resources.OpenRecord1CErrorNotExist;
      
      if (externalEntityLink.IsDeleted == true)
        return Integration1CDemo.Resources.OpenRecord1CErrorIsDelete;
      
      try
      {
        var link = Sungero.ExternalSystem.PublicFunctions.Module.Remote.GetEntityLink(externalEntityLink.ExtEntityId, externalEntityLink.ExtEntityType);
        Hyperlinks.Open(link);
        
        return null;
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Demo1C.OpenEntityIn1CFor. An error occured while getting the link to external entity. DocumentId = {0}", ex, document.Id);
        return Integration1CDemo.Resources.OpenRecord1CError;
      }
    }
  }
}