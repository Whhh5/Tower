using System;
using System.Collections;
using System.Collections.Generic;
using B1;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;


public class ChangeBloodData
{
    public int ChangeValue = 0;
    public WorldObjectBaseData Initiator = null;
    public WorldObjectBaseData Target = null;
    public EDamageType EDamageType = EDamageType.Physical;
}
public abstract class WorldObjectBaseData : PersonData
{
    protected WorldObjectBaseData(int f_index, int f_ChunkIndex) : base(f_index)
    {
        CurrentIndex = f_ChunkIndex;
        CurrentMapKey = WorldMapManager.Ins.CurrentMapKey;
    }

    public int CurrentMapKey { get; private set; }
    public int CurrentIndex { get; protected set; }
    public void SetCurrentChunkIndex(int f_ToIndex)
    {
        CurrentIndex = f_ToIndex;
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

        if (WorldMapManager.Ins.TryGetChunkData(CurrentIndex, out var targetChunk))
        {
            SetPosition(targetChunk.PointUp);
        }

        if (GTools.TableMgr.TryGetColorByObjectType(ObjectType, out var color))
        {
            SetColor(color);
        }
    }
    public override void OnUnLoad()
    {
        m_BuffDic.Clear();
        WorldMapManager.Ins.RemoveChunkElement(this);
        base.OnUnLoad();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        UpdateBuff();
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 攻击 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public virtual int HarmBase => 12;
    // 暴击率 0 - 1
    public float CriticalChance { get; private set; } = 0.2f;
    // 暴击倍数, 相当于攻击的多少倍 1 - n
    public float CriticalMultiple { get; private set; } = 2;

    /// <summary>
    /// 获取当前可攻击状态
    /// </summary>
    /// <returns></returns>
    public virtual ResultData<string> IsHitConditoin()
    {
        var result = new ResultData<string>(EResult.Succeed);
        var rangeVallue = GTools.MathfMgr.GetRandomValue(0, 1.0f);
        if (rangeVallue < 0.2f)
        {
            result.SetData($"-N-", EResult.Defeated);
        }
        return result;
    }

    public virtual void AttackTarget()
    {
        ExecuteGainAsync(EGainView.Launch); 
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
    public int MaxBlood { get; private set; } = 523;
    public int CurrentMagic { get; protected set; } = 300;
    public int MaxMagic { get; private set; } = 653;
    public float MagicPercent => (float)CurrentMagic / MaxMagic;
    public virtual int ChangeBlood(ChangeBloodData f_Data)
    {
        var value = CurrentBlood + f_Data.ChangeValue;

        CurrentBlood = Mathf.Clamp(value, 0, MaxBlood);

        if (value <= 0)
        {
            EntityDied();
        }
        return CurrentBlood;
    }
    public virtual int ChangeMagic(int f_Increment)
    {
        var value = CurrentMagic + f_Increment;

        CurrentMagic = Mathf.Clamp(value, 0, MaxMagic);


        return CurrentMagic;
    }
    public virtual void EntityDied()
    {
        SetPersonStatus(EPersonStatusType.Die, EAnimatorStatus.Stop);
        WorldMapManager.Ins.RemoveChunkElement(this);
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
    protected Dictionary<EGainView, Dictionary<EGainType, GainBaseData>> m_CurGainList = new();
    // 触发增益
    public void ExecuteGainAsync(EGainView f_GainType)
    {
        if (m_CurGainList.TryGetValue(f_GainType, out var list))
        {
            foreach (var item in list)
            {
                item.Value.StartExecute();
            }
        }
    }

    public void AddGainAsync(EGainType f_GainType, WorldObjectBaseData f_Initiator)
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
                list.Add(f_GainType, gainData);
            }
        }
    }
}

public class WorldObjectBase : Person, IUpdateBase
{
    public override EUpdateLevel UpdateLevel => EUpdateLevel.Level1;
    public WorldObjectBaseData WorldObjectBaseData => GetData<WorldObjectBaseData>();
    public override void OnUpdate()
    {
        base.OnUpdate();


        if (Input.GetMouseButtonUp(0))
        {

        }
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
}
