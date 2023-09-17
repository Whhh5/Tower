using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class UIItemBaseData<T> : UnityObjectData
    where T : class
{
    public UIItemBaseData() : base(0)
    {
    }

    public T Data;
    public int ListIndex;

    public override EWorldObjectType ObjectType => EWorldObjectType.None;

    public virtual void UpdateItemData(T f_Data, Transform f_Parent)
    {
        Data = f_Data;
        SetParent(f_Parent != null ? f_Parent : Parent);
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(this));
    }
    public override void AfterLoad()
    {
        base.AfterLoad();
        this.Initialization();
    }
}
public abstract class UIItemBase : ObjectPoolBase
{

}
