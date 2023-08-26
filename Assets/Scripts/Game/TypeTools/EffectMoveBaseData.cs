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


/// <summary>
/// 跟踪目标
/// </summary>
/// <typeparam name="T"></typeparam>
public class Effect_Track_Line<T> : EffectMoveBaseData
    where T : TestTimeLineData
{
    public Effect_Track_Line(T f_EffectData, float f_UnitSpeed = 1) : base(f_EffectData, f_EffectData.WorldPosition, f_EffectData.TargetEnemy.CentralPoint, f_UnitSpeed)
    {
        m_Initiator = f_EffectData.Initiator;
        m_TargetEnemy = f_EffectData.TargetEnemy;
    }
    public T Effect = null;
    private WorldObjectBaseData m_Initiator = null;
    private WorldObjectBaseData m_TargetEnemy = null;
    public override bool GetExecuteCondition()
    {
        var result = Vector3.Magnitude(TargetPosition - CurPosition) > 0.001f;
        return result;
    }

    public override Vector3? GetPosition()
    {
        return m_TargetEnemy != null ? m_TargetEnemy.CentralPoint : TargetPosition;
    }
}
/// <summary>
/// 目标点列表寻路
/// </summary>
/// <typeparam name="T"></typeparam>
public class Effect_Track_Points<T> : EffectMoveBaseData
   where T : EntityEffectBaseData
{
    public Effect_Track_Points(T f_EffectData, List<WorldObjectBaseData> f_PathList, float f_UnitSpeed = 1) : base(f_EffectData, f_EffectData.WorldPosition, f_EffectData.WorldPosition, f_UnitSpeed)
    {
        m_CurPathListIndex = 0;
        m_Initiator = f_EffectData.Initiator;
        m_TargetEnemy = f_PathList[m_CurPathListIndex];
        m_MovePathList = f_PathList;
        Effect = f_EffectData;
    }
    public T Effect = null;
    private WorldObjectBaseData m_Initiator = null;
    private WorldObjectBaseData m_TargetEnemy = null;

    private List<WorldObjectBaseData> m_MovePathList = null;
    private int m_CurPathListIndex = 0;

    public override bool GetExecuteCondition()
    {
        var result = true;
        if (Vector3.Magnitude(CurPosition - TargetPosition) < 0.001f)
        {
            Effect.ExecuteEffect(m_TargetEnemy);
            m_CurPathListIndex++;
            if (m_CurPathListIndex < m_MovePathList.Count)
            {
                m_TargetEnemy = m_MovePathList[m_CurPathListIndex];
            }
            else
            {
                result = false;
            }
        }



        return result;
    }

    public override Vector3? GetPosition()
    {
        return m_TargetEnemy != null ? m_TargetEnemy.CentralPoint : TargetPosition;
    }
}