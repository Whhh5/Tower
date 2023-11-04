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
    //                                catalogue -- �ο���ƪ
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
    //                                catalogue -- �������� ƪ
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




















