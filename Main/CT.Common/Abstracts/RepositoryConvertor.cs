using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CT.Common.Abstracts
{
    /// <summary>
    /// An abstract class that converts between T entity & model
    /// </summary>
    /// <typeparam name="Entity">the entity type</typeparam>
    /// <typeparam name="DTO">the model type</typeparam>
    public abstract class RepositoryConvertor<Entity, DTO>
        where Entity : class, new()
        where DTO : class, new()
    {
        protected virtual Entity ConvertToEntity(DTO dto)
        {
            Entity entity = dto as Entity;
            return entity;
        }
        protected virtual DTO ConvertToDTO(Entity entity)
        {
            DTO dto = entity as DTO;
            return dto;
        }
    }
}
