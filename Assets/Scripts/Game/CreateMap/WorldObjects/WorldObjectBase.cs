using System;
using System.Collections;
using System.Collections.Generic;
using B1;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;


public class ChangeBloodData
{
    public int ChangeValue = 0;
    public WorldObjectBaseData Initiator = null;
    public WorldObjectBaseData Target = null;
    public EDamageType EDamageType = EDamageType.Physical;
}
public abstract class WorldObjectBaseData : DependChunkData
{
    public virtual string ObjectName { get; }
    protected WorldObjectBaseData() : base()
    {
        CurrentBlood = MaxBlood;
    }
    public virtual void Initialization(int f_index, int f_ChunkIndex)
    {
        SetCurrentChunkIndex(f_ChunkIndex);
    }

    public abstract EEntityType EntityType { get; }
    public WorldObjectBase WorldObjectTarget => GetCom<WorldObjectBase>();
    public virtual EHeroVocationalType EntityVocationalType { get; } = EHeroVocationalType.Warrior; 
    private bool m_CurObjBehaviorStatus = false;
    public bool GetObjBehaviorStatus()
    {
        return m_CurObjBehaviorStatus;
    }
    public void SetObjBehaviorStatus(bool f_IsActiveStatus)
    {
        m_CurObjBehaviorStatus = f_IsActiveStatus;
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 生命周期 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public override void AfterLoad()
    {
        base.AfterLoad();
        Resurgence();
        InitAnimatorParams();
        SetPersonStatus(EPersonStatusType.Idle);
    }
    public override void OnUnLoad()
    {
        GTools.WorldWindowMgr.RemoveBloodHint(this);
        base.OnUnLoad();
    }
    // 复活
    public virtual void Resurgence()
    {
        SetPersonStatus(EPersonStatusType.Entrance);
        SetAllElementColorAlpha(1);
        return;
        WorldWindowMgr.Ins.UpdateBloodHint(this);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        UpdateBuff();
        UpdateGain();
        UpdateAnimator();
    }
    public virtual void Death()
    {
        m_BuffDic.Clear();
        GTools.WorldWindowMgr.RemoveBloodHint(this);
        GTools.CreateMapNew.ClearChunkElement(this);
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 攻击 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public virtual int HarmBase => 12;
    protected float m_AddHarm = 0;
    public int CurHarm => Mathf.Clamp(Mathf.FloorToInt((1 + m_AddHarm) * HarmBase), 1, 200);
    // 暴击率 0 - 1
    public float CriticalChance { get; private set; } = 0.2f;
    // 暴击倍数, 相当于攻击的多少倍 1 - n
    public float CriticalMultiple { get; private set; } = 2;

    public void ChangeHarm(float f_Ratio)
    {
        m_AddHarm += f_Ratio;
    }
    /// <summary>
    /// 获取当前可攻击状态
    /// </summary>
    /// <returns></returns>
    public virtual ResultData<string> IsHitConditoin()
    {
        var result = new ResultData<string>(EResult.Succeed);
        var rangeVallue = GTools.MathfMgr.GetRandomValue(0, 1.0f);
        if (rangeVallue < 0.0f)
        {
            result.SetData($"miss", EResult.Defeated);
        }
        return result;
    }

    public virtual void AttackTarget()
    {
        ExecuteGainAsync(EBuffView.Launch);
    }
    public virtual void SkillTarget()
    {

    }



    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 血量蓝量 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public int CurrentBlood { get; private set; } = 400;
    private float m_AddMaxBlood = 0;
    public virtual int MaxBloodBase { get; private set; } = 523;
    public int MaxBlood => Mathf.Clamp(Mathf.CeilToInt((1 + m_AddMaxBlood) * MaxBloodBase), 100, 5000);
    public int CurrentMagic { get; protected set; } = 300;
    public virtual int AtkAddMagic => 30;
    public virtual int MaxMagic { get; private set; } = 653;
    public float MagicPercent => (float)CurrentMagic / MaxMagic;
    public virtual int DefenceBase => 20;
    private float m_AddDefence = 0;
    public int CurDefence => Mathf.Clamp(Mathf.CeilToInt(DefenceBase * (1 + m_AddDefence)), 1, 300);
    public virtual int ChangeBlood(ChangeBloodData f_Data)
    {
        var value = CurrentBlood + f_Data.ChangeValue;

        CurrentBlood = Mathf.Clamp(value, 0, MaxBlood);

        if (value <= 0)
        {
            SetPersonStatus(EPersonStatusType.Die, EAnimatorStatus.Stop);
            Death();
        }
        return CurrentBlood;
    }
    public virtual int ChangeMagic(int f_Increment)
    {
        var value = CurrentMagic + f_Increment;

        CurrentMagic = Mathf.Clamp(value, 0, MaxMagic);

        return CurrentMagic;
    }
    public virtual void ResetMagic()
    {
        CurrentMagic = 0;
    }
    public virtual bool IsPassSkill()
    {
        return MagicPercent >= 1;
    }
    public virtual void ChangeMaxBlood(float f_Ratio)
    {
        m_AddMaxBlood += f_Ratio;
    }
    public virtual void ChangeDefence(float f_Ratio)
    {
        m_AddDefence += f_Ratio;
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- buff 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    // buff 列表
    [SerializeField] protected Dictionary<EBuffType, Effect_BuffBaseData> m_BuffDic = new();
    protected ListStack<EBuffType> m_AddBuffDic = new("", 5);
    protected ListStack<EBuffType> m_RemoveBuffDic = new("", 5);


    // 添加 buff
    public void AddBuffAsync(EBuffType f_BuffID, WorldObjectBaseData f_Initiator)
    {
        if (m_BuffDic.TryGetValue(f_BuffID, out var IBuffBase))
        {
            IBuffBase.AddAsync(1);
        }
        else if (TableMgr.Ins.TryGetBuffInfo(f_BuffID, out var buffInfo))
        {
            var buffData = buffInfo.CreateBuffData(f_Initiator, this);
            buffData.StartExecute();
            m_BuffDic.Add(f_BuffID, buffData);
        }
    }

    // 移除 buff
    public void RemoveBuffAsync(EBuffType f_BuffID)
    {
        if (m_BuffDic.ContainsKey(f_BuffID))
        {
            m_BuffDic.Remove(f_BuffID);
        }
    }

    // 清除所有 buff
    public void ClearBuffsAsync()
    {
        m_BuffDic.Clear();
    }
    public void UpdateBuff()
    {

    }
    public Dictionary<EBuffType, Effect_BuffBaseData> GetBuff()
    {
        return m_BuffDic;
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 增益篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    // Gain
    protected Dictionary<EBuffView, Dictionary<EGainType, EntityGainBaseData>> m_CurGainList = new();
    // 触发增益
    public void ExecuteGainAsync(EBuffView f_GainType)
    {
        if (m_CurGainList.TryGetValue(f_GainType, out var list))
        {
            foreach (var item in list)
            {
                item.Value.Execute();
            }
        }
    }

    public void AddGain(EGainType f_GainType, WorldObjectBaseData f_Initiator)
    {
        if (TableMgr.Ins.TryGetGainInfo(f_GainType, out var gainInfo))
        {
            if (!m_CurGainList.TryGetValue(gainInfo.GainView, out var list))
            {
                list = new();
                m_CurGainList.Add(gainInfo.GainView, list);
            }
            if (list.TryGetValue(f_GainType, out var gain))
            {
                gain.Reset();
            }
            else
            {
                var gainData = gainInfo.CreateGain(f_Initiator, this);
                gainData.StartExecute();
                list.Add(f_GainType, gainData);
            }
        }
    }
    public void RemoveGain(EGainType f_GainType)
    {
        if (!TableMgr.Ins.TryGetGainInfo(f_GainType, out var gainInfo))
        {
            return;
        }
        if (!m_CurGainList.TryGetValue(gainInfo.GainView, out var list))
        {
            return;
        }
        if (list.TryGetValue(f_GainType, out var gainData))
        {
            list.Remove(f_GainType);
            gainData.StopExecute();
            if (list.Count == 0)
            {
                m_CurGainList.Remove(gainInfo.GainView);
            }
        }
    }

    public void UpdateGain()
    {

    }


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 更新属性 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public void UpdateAddAttributes(IncubatorAttributeInfo f_AttributeInfos)
    {
        m_AddAtkSpeed = Mathf.FloorToInt(f_AttributeInfos.AtkSpeedRatio);
        m_AddDefence = Mathf.FloorToInt(f_AttributeInfos.DefenceRatio);
        m_AddMaxBlood = Mathf.FloorToInt(f_AttributeInfos.BloodRatio);
        m_AddHarm = Mathf.FloorToInt(f_AttributeInfos.HarmRatio);
    }


    public virtual void SetPersonStatus(EPersonStatusType f_ToStatus, EAnimatorStatus f_AnimaStatus = EAnimatorStatus.Loop)
    {
        if (CurStatus != f_ToStatus)
        {
            CurStatus = f_ToStatus;
            CurAnimaNormalizedTime = 0;
            AnimaStatus = f_AnimaStatus;
            UpdateCallbackStatus();
            PlayerAnimation();
        }
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 动画篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    class AnomatorCallback
    {
        public float NormalizedTime = 0;
        public Action Action = null;
        public bool Status = false;
        public void Init()
        {
            Status = true;
        }
        public void Invok()
        {
            Status = false;
            Action?.Invoke();
        }
    }
    // 当前动画播放进度
    private float m_CurAnimaNormalizedTime = 0;
    public float CurAnimaNormalizedTime
    {
        get => m_CurAnimaNormalizedTime;
        set => m_CurAnimaNormalizedTime = Mathf.Clamp01(value);
    }

    // 当前动画播放速度
    public virtual float AtkSpeedBase { get; } = 1;
    protected float m_AddAtkSpeed = 0;
    public float CurAtkSpeed => AtkSpeedBase * (1 + m_AddAtkSpeed);

    public virtual int AtkRangeBase { get; } = 1;
    protected int m_AddAtkRange = 0;
    public int CurAtkRange => AtkRangeBase + m_AddAtkRange;


    public float CurAnimaSpeed => Mathf.Clamp(AtkSpeedBase * (1 + m_AddAtkSpeed), 0.1f, 10);
    public void ChangeReleaseSpeed(float f_Value)
    {
        m_AddAtkSpeed += f_Value;
    }
    // 移动速度
    protected virtual float m_MoveSpeedBase { get; } = 1;
    protected float m_AddMoveSpeed = 0;
    public float CurMoveSpeed => Mathf.Clamp(m_MoveSpeedBase + m_AddMoveSpeed, 0.1f, 10);
    public void ChangeMoveSpeed(float f_Value)
    {
        var value = f_Value * m_MoveSpeedBase;
        m_AddMoveSpeed += value;
    }
    // 动画状态
    public EAnimatorStatus AnimaStatus = EAnimatorStatus.None;
    private Dictionary<int, AnomatorCallback> m_DicAnimaCallBack = new();
    private float CurAnimationTime => WorldObjectTarget != null ? WorldObjectTarget.CurAnimationTime : 1;
    public void InitAnimatorParams()
    {
        int index = 0;
        AnimaStatus = EAnimatorStatus.Loop;
        m_DicAnimaCallBack = new()
        {
            {
                index++,
                new()
                {
                    NormalizedTime = 0.0f,
                    Status = true,
                    Action = AnimatorCallback000,
                }
            },
            {
                index++,
                new()
                {
                    NormalizedTime = 0.2f,
                    Status = true,
                    Action = AnimatorCallback020,
                }
            },
            {
                index++,
                new()
                {
                    NormalizedTime = 0.5f,
                    Status = true,
                    Action = AnimatorCallback050,
                }
            },
            {
                index++,
                new()
                {
                    NormalizedTime = 0.7f,
                    Status = true,
                    Action = AnimatorCallback070,
                }
            },
            {
                index++,
                new()
                {
                    NormalizedTime = 1.0f,
                    Status = true,
                    Action = AnimatorCallback100,
                }
            },
        };
    }
    public void UpdateCallbackStatus()
    {
        foreach (var item in m_DicAnimaCallBack)
        {
            item.Value.Init();
        }
    }
    public void PlayerAnimation(bool f_IsForce = false)
    {
        if (WorldObjectTarget != null && !f_IsForce)
        {
            WorldObjectTarget.PlayerAnimation();
        }
    }
    public virtual string GetCurrentAnimationName()
    {
        return $"{CurStatus}";
    }
    private void UpdateAnimator()
    {
        switch (AnimaStatus)
        {
            case EAnimatorStatus.None:
                break;
            case EAnimatorStatus.One:
                {
                    if (CurAnimaNormalizedTime == 1)
                    {
                        SetPersonStatus(EPersonStatusType.Idle);
                    }
                    UpdateCurAnimaNormalizedTime();
                }
                break;
            case EAnimatorStatus.Loop:
                {
                    UpdateCurAnimaNormalizedTime();
                }
                break;
            case EAnimatorStatus.Stop:
                {
                    if (CurAnimaNormalizedTime != 1)
                    {
                        UpdateCurAnimaNormalizedTime();
                    }
                }
                break;
            default:
                break;
        }
        PlayerAnimation();
    }

    private void UpdateCurAnimaNormalizedTime()
    {
        // 目标进度
        var speed = 1.0f;
        if (CurStatus is EPersonStatusType.Attack or EPersonStatusType.Skill)
        {
            speed = CurAnimaSpeed;
        }
        else if (CurStatus is EPersonStatusType.Walk)
        {
            speed = CurMoveSpeed;
        }
        var value = CurAnimaNormalizedTime + UpdateDelta * speed / CurAnimationTime;
        if (CurStatus == EPersonStatusType.Die && GetType() == typeof(Entity_Incubator1Data))
        {
            Log("");
        }
        // 动画回调
        foreach (var item in m_DicAnimaCallBack)
        {
            if (item.Value.Status && item.Value.NormalizedTime <= value)
            {
                item.Value.Invok();
            }
        }

        // 更新进度
        if (value >= 1)
        {
            CurAnimaNormalizedTime = AnimaStatus == EAnimatorStatus.Loop ? value - 1 : 1;
            UpdateCallbackStatus();
        }
        else
        {
            CurAnimaNormalizedTime = value;
        }


        //AnimationClip clip = 
        //clip.events
    }

    public virtual void AnimatorCallback000()
    {

    }
    public virtual void AnimatorCallback020()
    {

    }
    public virtual void AnimatorCallback050()
    {

    }
    public virtual void AnimatorCallback070()
    {

    }
    public virtual void AnimatorCallback100()
    {
        switch (CurStatus)
        {
            case EPersonStatusType.Entrance:
                {
                    SetPersonStatus(EPersonStatusType.Idle);
                }
                break;
            case EPersonStatusType.Die:
                {

                }
                break;
            default:
                break;
        }
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 鼠标范围检测
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--

    public virtual void OnMouseEnterRange()
    {
        
    }
    public virtual void OnMouseExitRange()
    {
        
    }
    public float AllElementAlpha = 1.0f;
    public void SetAllElementColorAlpha(float f_Alpha)
    {
        AllElementAlpha = f_Alpha;
        if (WorldObjectTarget != null)
        {
            WorldObjectTarget.SetAllElementColorAlpha();
        }
    }

}

public class WorldObjectBase : DependChunk, IUpdateBase
{
    public WorldObjectBaseData ObjData => GetData<WorldObjectBaseData>();
    public override EUpdateLevel UpdateLevel => EUpdateLevel.Level1;
    public WorldObjectBaseData WorldObjectBaseData => GetData<WorldObjectBaseData>();

    [SerializeField]
    private float m_Range = 10;
    [SerializeField]
    private bool m_IsEnter = false;
    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        await base.OnStartAsync(f_Params);

        PlayerAnimation();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();


        Vector3 screenPoint = Camera.main.WorldToScreenPoint(ObjData.WorldPosition);
        var isEnter = Vector3.SqrMagnitude(screenPoint - Input.mousePosition) > m_Range;
        if (!m_IsEnter && isEnter)
        {
            ObjData.OnMouseEnterRange();
        }
        else if (m_IsEnter && !isEnter)
        {
            ObjData.OnMouseExitRange();
        }
    }
    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
    }
    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
        if (CurAnim != null)
        {
            CurAnim.StopPlayback();
        }
        m_AnimatorGraph = PlayableGraph.Create();
    }
    private void OnMouseDown()
    {

    }
    private void OnMouseEnter()
    {
        UISelectHeroInfo.Ins.SetData(WorldObjectBaseData);
    }

    private void OnMouseExit()
    {
        UISelectHeroInfo.Ins.SetData(null);
    }

    public void SetAllElementColorAlpha()
    {
        var coms = GetComponentsInChildren<SpriteRenderer>();
        if (coms != null)
        {
            foreach (var item in coms)
            {
                var color = item.color;
                color.a = WorldObjectBaseData.AllElementAlpha;
                item.color = color;
            }
        }
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 动画篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    [SerializeField]
    private Animator m_CurAnim = null;
    public Animator CurAnim => m_CurAnim != null ? m_CurAnim : GetComponent<Animator>();
    public float CurAnimationTime
    {
        get
        {
            var value = 1.0f;
            if (CurAnim != null)
            {
                var curArr = CurAnim.GetCurrentAnimatorClipInfo(0);
                value = curArr != null && curArr.Length > 0 ? curArr[0].clip.length : 1;
                value = value > 0 ? value : 1;
            }
            return value;
        }
    }
    private bool TryGetAnimationClip(string f_Name, out AnimationClip f_Clip, int f_Layer = 0)
    {
        var clips = CurAnim.runtimeAnimatorController.animationClips;
        f_Clip = null;
        foreach (var item in clips)
        {
            var hashCode = item.GetHashCode();
            if (!item.name.Contains(f_Name))
            {
                continue;
            }
            //f_Clip = item;
            break;
        }
        return f_Clip != null;
    }
    private string m_LastAnimaClipName = "";
    private PlayableGraph m_AnimatorGraph;
    public void PlayerAnimation()
    {
        if (CurAnim != null)
        {
            var animaName = WorldObjectBaseData.GetCurrentAnimationName();

            CurAnim.Play(animaName, 0, WorldObjectBaseData.CurAnimaNormalizedTime);


            //if (m_LastAnimaClipName != "" && m_LastAnimaClipName != animaName
            //    && TryGetAnimationClip(m_LastAnimaClipName, out var lastAnim) 
            //    && TryGetAnimationClip(animaName, out var curAnima))
            //{
            //    var animOutput = AnimationPlayableOutput.Create(m_AnimatorGraph, "AnimationOutout", CurAnim);

            //    var mixerPlayable = AnimationMixerPlayable.Create(m_AnimatorGraph, 2);

            //    var clipPlayableA = AnimationClipPlayable.Create(m_AnimatorGraph, lastAnim);
            //    var clipPlayableB = AnimationClipPlayable.Create(m_AnimatorGraph, curAnima);


            //    m_AnimatorGraph.Connect(clipPlayableA, 0, mixerPlayable, 0);
            //    m_AnimatorGraph.Connect(clipPlayableB, 0, mixerPlayable, 1);

            //    animOutput.SetSourcePlayable(mixerPlayable);

            //    mixerPlayable.SetInputWeight(0, 1);
            //    mixerPlayable.SetInputWeight(1, 1);

            //    m_AnimatorGraph.Play();

            //}
            m_LastAnimaClipName = animaName;
        }
    }
}
