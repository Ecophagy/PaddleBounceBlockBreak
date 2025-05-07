using System;

namespace PaddleBounceBlockBreak.Entities;

public class Entity
{
    public Guid EntityId { get; } = Guid.NewGuid();
    public string EntityName { get; } // Human-readable, for debugging

    public Entity(string entityName)
    {
        EntityName = entityName;
    }
}