using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EntityCommandType
{
}
public class EntityCommand<T>
{
    private T data;
    private ListQueue<Action<T>> m_List;

    public EntityCommand(T f_Target, int f_CacheCount)
    {
        data = f_Target;
        m_List = new("√¸¡Ó∂”¡–", f_CacheCount);
    }

    public void AddCommand()
    {
    }

    public void Invoke()
    {
    }
}

public abstract class VirtualEntityData : UnityObjectData
{
    protected VirtualEntityData(int f_Index) : base(f_Index)
    {
    }
}
public abstract class VirtualEntity : ObjectPoolBase, IExecute
{
    public abstract UniTask StartExecute();

    public abstract UniTask StopExecute();
}
