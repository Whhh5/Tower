using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity_FormationBaseData : UnityObjectData
{
    public Entity_FormationBaseData() : base(0)
    {
    }


    public virtual ELayer AttackLayer => ELayer.Enemy;
    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;
    public FormationData FormationData = null;
    public virtual int CurHarm => 30;
    public void SetFormationData(FormationData f_Data)
    {
        FormationData = f_Data;
    }
}
public abstract class Entity_FormationBase : ObjectPoolBase
{
   
}
