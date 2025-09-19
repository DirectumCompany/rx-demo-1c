using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Demo1C.ExternalEntityLink;
using Sungero.Domain.Shared;

namespace Sungero.Demo1C.Server
{
  partial class ExternalEntityLinkFunctions
  {
    /// <summary>
    /// Получить связь сущности с внешней системой (1С).
    /// </summary>
    /// <param name="entity">Запись Directum RX.</param>
    /// <returns>Связь сущности с внешней системой. Если не найдена, то null.</returns>
    [Public, Remote(IsPure = true)]
    public static IExternalEntityLink GetForEntityIn1C(Sungero.Domain.Shared.IEntity entity)
    {
      var entityExtLinks = Sungero.Commons.PublicFunctions.ExternalEntityLink
        .GetExternalByEntityType(entity.TypeDiscriminator.GetTypeByGuid());
      
      var extSystemId = Sungero.ExternalSystem.PublicFunctions.Module.GetId();     
      var extEntityLink = entityExtLinks.FirstOrDefault(x => x.EntityId == entity.Id && x.ExtSystemId == extSystemId);
      
      return ExternalEntityLinks.As(extEntityLink);
    }
    
    /// <summary>
    /// Создать связь сущности с внешней системой (1С).
    /// </summary>
    /// <param name="entity">Сущность.</param>
    /// <param name="externalEntityId">Идентификатор сущности в 1С.</param>
    /// <param name="externalEntityType">Тип сущности в 1С.</param>
    [Public]
    public static void CreateNew(Sungero.Domain.Shared.IEntity entity, string externalEntityId, string externalEntityType)
    {
      var externalEntityLink = ExternalEntityLinks.Create();
      externalEntityLink.EntityId = entity.Id;
      externalEntityLink.EntityType = entity.GetType().GetOriginalType().GetTypeGuid().ToString();
      externalEntityLink.ExtEntityId = externalEntityId;
      externalEntityLink.ExtEntityType = externalEntityType;
      externalEntityLink.ExtSystemId = Sungero.ExternalSystem.PublicFunctions.Module.GetId();
      externalEntityLink.Save();
    }
  }
}