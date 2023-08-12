using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EffectMoveBaseData : IUpdateBase, IExecute
{
    public EffectMoveBaseData(EntityEffectBaseData f_EffectData, Vector3 f_StartPosition, Vector3 f_EndPosition, float f_UnitSpeed = 1)
    {
        Index = f_EffectData.Index;
        OnEnable = true;
        EffectData = f_EffectData;
        StartPosition = f_StartPosition;
        UnitSpeed = f_UnitSpeed;
        TargetPosition = f_EndPosition;
    }
    public int Index;
    public bool OnEnable = true;
    public EntityEffectBaseData EffectData = null;
    public Vector3 StartPosition = Vector3.zero;
    public Vector3 TargetPosition = Vector3.zero;
    public Vector3 CurPosition => EffectData.WorldPosition;
    public float UnitSpeed = 1.0f;

    public int UpdateLevelID { get; set; }

    public EUpdateLevel UpdateLevel => EUpdateLevel.Level2;


    public abstract Vector3? GetPosition();
    public abstract bool GetExecuteCondition();

    public void UpdatePosition()
    {
        var value = GetPosition() ?? TargetPosition;
        TargetPosition = value;


        Vector3 targetPos = Vector3.MoveTowards(EffectData.WorldPosition, value, Time.deltaTime * UnitSpeed);
        EffectData.SetForward(targetPos - EffectData.WorldPosition);
        EffectData.SetPosition(targetPos);
    }

    public void OnUpdate()
    {
        UpdatePosition();
        OnEnable = GetExecuteCondition();
        if (!OnEnable)
        {
            GTools.RunUniTask(StopExecute());
        }
    }

    public async UniTask StartExecute()
    {
        EffectData.SetPosition(StartPosition);
        await ILoadPrefabAsync.LoadAsync(EffectData);
        GTools.LifecycleMgr.AddUpdate(this);
    }

    public async UniTask StopExecute()
    {
        ILoadPrefabAsync.UnLoad(EffectData);
        GTools.LifecycleMgr.RemoveUpdate(this);
    }
}