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
    public Entity EntityTarget => GetCom<Entity>();
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 参考点篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public Vector3 CentralPoint => EntityTarget != null && EntityTarget.CentralPoint != null
        ? EntityTarget.CentralPoint.position
        : WorldPosition;

    public Vector3 BeHitPoint => EntityTarget != null && EntityTarget.BeHitPoint != null
        ? EntityTarget.BeHitPoint.position
        : WorldPosition;

    public Vector3 BuffPoint => EntityTarget != null && EntityTarget.BuffPoint != null
        ? EntityTarget.BuffPoint.position
        : WorldPosition;

    public Vector3 EffectPoint => EntityTarget != null && EntityTarget.EffectPoint != null
        ? EntityTarget.EffectPoint.position
        : WorldPosition;

    public Vector3 TrailPoint => EntityTarget != null && EntityTarget.TrailPoint != null
        ? EntityTarget.TrailPoint.position
        : WorldPosition;

    public Vector3 AttackSpeedPoint => EntityTarget != null && EntityTarget.AttackSpeedPoint != null
        ? EntityTarget.AttackSpeedPoint.position
        : WorldPosition;
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


    // point
    [SerializeField] private Transform m_CentralPoint = null;
    public Transform CentralPoint => m_CentralPoint;
    [SerializeField] private Transform m_BeHitPoint = null;
    public Transform BeHitPoint => m_BeHitPoint;
    [SerializeField] private Transform m_BuffPoint = null;
    public Transform BuffPoint => m_BuffPoint;
    [SerializeField] private Transform m_EffectPoint = null;
    public Transform EffectPoint => m_EffectPoint;
    [SerializeField] private Transform m_TrailPoint = null;
    public Transform TrailPoint => m_TrailPoint;
    [SerializeField] protected Transform m_AttackSpeedPoint = null;
    public Transform AttackSpeedPoint => m_AttackSpeedPoint;
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




















