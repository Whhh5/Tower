using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect_BuffBaseData : UnityObjectData
{
    public Effect_BuffBaseData() : base(0)
    {
    }

    public abstract override AssetKey AssetPrefabID { get; }

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    protected int m_CurSupCount = 0;
    protected int m_SuperpositionCount = 1;
    protected float m_StartTime = 0;
    public int SuperpositionCount => m_SuperpositionCount;

    protected int m_Id = 0;
    public int ID => m_Id;
    protected string m_Name = "";
    public string Name => m_Name;
    protected ushort m_Level = 0;
    public ushort Level => m_Level;
    protected virtual int HarmBase { get; } = 1;

    protected virtual float IntervalTime { get; } = 0.0f;
    protected virtual float UnitTime { get; } = 3.0f;

    protected WorldObjectBaseData m_Initiator = null;
    protected WorldObjectBaseData m_Target = null;


    // 当前间隔时间
    public float CurIntervalTime => IntervalTime / Mathf.Clamp(m_SuperpositionCount * m_Level * 0.1f, 1, 3);
    // 当前伤害数值
    public int CurHarmValue => HarmBase * m_Level * (m_SuperpositionCount - m_CurSupCount);
    // 当前 层 CommandQueue 持续时间进度
    public float CurSupRatio = 0.0f;
    // 当前 CommandQueue 持续时间总进度
    public float CurRatio = 0.0f;




    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 生命周期篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public override bool IsUpdateEnable => true;
    public override void OnUpdate()
    {
        base.OnUpdate();

        SetPosition(GetTargetPoint());
    }
    public abstract Vector3 GetTargetPoint();
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 效果更新篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public abstract void ExecuteResultAsync(int f_Value, float f_Ratio);
    public virtual async void StartExecute()
    {
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(this));
        m_StartTime = GTools.CurTime;


        while (m_CurSupCount < m_SuperpositionCount && GTools.UnityObjectIsActive(m_Target))
        {
            await GTools.WaitSecondAsync(CurIntervalTime);

            var timed = GTools.CurTime - m_StartTime;
            m_CurSupCount = Mathf.FloorToInt(timed / UnitTime);
            CurSupRatio = timed % UnitTime / UnitTime;
            CurRatio = timed / (UnitTime * m_SuperpositionCount);
            ExecuteResultAsync(CurHarmValue, CurSupRatio);

        }

        StopExecute();
    }

    public virtual void StopExecute()
    {
        m_SuperpositionCount = 0;
        m_CurSupCount = 0;
        m_Level = 0;
        m_StartTime = 0;
        CurSupRatio = 0;
        CurRatio = 0;
        ILoadPrefabAsync.UnLoad(this);
    }
    public virtual void Initialization(WorldObjectBaseData f_Original, WorldObjectBaseData f_Target)
    {
        m_Target = f_Target;
        m_Initiator = f_Original;
    }
    public void ReleaseAsync()
    {
        m_Target = null;
        m_Initiator = null;
    }
    public void AddAsync(ushort f_Level)
    {
        if (f_Level > m_Level)
        {
            m_Level = f_Level;

            m_CurSupCount = 0;
        }


        if (m_SuperpositionCount++ == 0)
        {
            StartExecute();
        }
    }

    public void RemoveAsync()
    {
        m_SuperpositionCount = Mathf.Max(m_SuperpositionCount - 1, 0);
    }

}
public abstract class Effect_BuffBase : ObjectPoolBase
{
    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();

        ParticleSystem[] list = transform.GetComponentsInChildren<ParticleSystem>();
        if (list != null && list.Length > 0)
        {
            foreach (var item in list)
            {
                item.Play();
            }
        }
    }
}
