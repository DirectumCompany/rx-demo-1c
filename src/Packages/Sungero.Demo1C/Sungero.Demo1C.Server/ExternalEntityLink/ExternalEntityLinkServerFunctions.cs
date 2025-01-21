using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Demo1C.ExternalEntityLink;

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
      return GetEntitiesIn1C(entity).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить связь сущности с внешней системой (1С).
    /// </summary>
    /// <param name="entity">Запись Directum RX.</param>
    /// <param name="extEntityType">Тип сущности в 1C.</param>
    /// <returns>Связь сущности с внешней системой. Если не найдена, то null.</returns>
    [Public, Remote(IsPure = true)]
    public static IExternalEntityLink GetForEntityIn1C(Sungero.Domain.Shared.IEntity entity, string extEntityType)
    {
      return GetEntitiesIn1C(entity).Where(x => x.ExtEntityType.Equals(extEntityType)).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить связи сущностей с внешней системой (1С).
    /// </summary>
    /// <param name="entity">Запись Directum RX.</param>
    /// <returns>Связи сущностей с внешней системой. Если не найдена, то null.</returns>
    private static IQueryable<IExternalEntityLink> GetEntitiesIn1C(Sungero.Domain.Shared.IEntity entity)
    {
      var typeGuid = entity.TypeDiscriminator.ToString();
      var extSystemId = Sungero.ExternalSystem.PublicFunctions.Module.GetId();
      
      return ExternalEntityLinks.GetAll()
        .Where(x => string.Equals(x.EntityType, typeGuid, StringComparison.OrdinalIgnoreCase) &&
               x.EntityId == entity.Id &&
               x.ExtSystemId == extSystemId);
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
      externalEntityLink.EntityType = entity.TypeDiscriminator.ToString();
      externalEntityLink.ExtEntityId = externalEntityId;
      externalEntityLink.ExtEntityType = externalEntityType;
      externalEntityLink.ExtSystemId = Sungero.ExternalSystem.PublicFunctions.Module.GetId();
      externalEntityLink.Save();
    }
  }
}