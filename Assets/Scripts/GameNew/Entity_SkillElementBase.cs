using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity_SkillElementBaseData : UnityObjectData
{
    public Entity_SkillElementBaseData() : base(0)
    {
    }


    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;
}
public abstract class Entity_SkillElementBase : ObjectPoolBase
{
    
}
