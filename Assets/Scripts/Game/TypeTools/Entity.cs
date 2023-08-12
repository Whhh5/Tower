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
    //                                catalogue -- �������� ƪ
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
    //                                catalogue -- ���ʵ��ص�ƪ
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




















