using B1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EntityData : UnityObjectData
{
    protected EntityData(int f_index) : base(f_index)
    {
        
    }
}

// base class
public abstract class Entity : ObjectPoolBase, IButton3DClick
{
    public EntityData EntityData => GetData<EntityData>();
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 生命周期 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 点击实体回调篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public virtual async UniTask OnClickAsync()
    {

    }

    public virtual async UniTask OnClick2Async()
    {

    }

}




















