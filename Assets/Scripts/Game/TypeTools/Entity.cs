using B1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChangeBloodData
{
    public int ChangeValue = 0;
    public EntityData Initiator = null;
    public EntityData Target = null;
    public EDamageType EDamageType = EDamageType.Physical;
}
public abstract class EntityData : UnityObjectData
{
    protected EntityData(int f_index) : base(f_index)
    {
        
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 生命周期 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    protected Dictionary<int, IEntityDestry> m_IEntityDestroyList = new();
    public override void AfterLoad()
    {
        base.AfterLoad();
        //foreach (var item in GTools.BuffMgr.BuffMap)
        //{
        //    var target = GTools.BuffMgr.GetBuff(item.Key, this);
        //    if (target.Result == EResult.Succeed)
        //    {
        //        m_BuffDic.Add(item.Key, target.Value);
        //    }
        //}
    }
    public override void OnUnLoad()
    {
        foreach (var item in m_IEntityDestroyList)
        {
            item.Value.CurEntityDestroyAsync();
        }

        m_BuffDic.Clear();

        base.OnUnLoad();
    }
    public override async void OnUpdate()
    {
        base.OnUpdate();

        var index = 0;
        var buffLocalPos = BuffPoint;
        Vector3 localPos = new Vector3(
            buffLocalPos.x > 0 ? 0.2f / buffLocalPos.x : 0,
            buffLocalPos.y,
            buffLocalPos.z > 0 ? 0.2f / buffLocalPos.y : 0);
        foreach (var item in m_BuffDic)
        {
            if (item.Value.CurRatio > 0)
            {
                if (!m_BuffHintDic.TryGetValue(item.Key, out var target))
                {
                    await GTools.AddDicElementAsync(m_BuffHintDic, item.Key, async () =>
                    {
                        target = await GTools.LoadWorldBuffHintAsync(item.Value);
                        return target;
                    });
                }

                var i = index++;
                var pos = buffLocalPos + new Vector3
                (
                    buffLocalPos.x * (localPos.x * i),
                    0,
                    buffLocalPos.z * (localPos.z * i)
                );
                target?.SetLocalPos(pos + WorldPosition);
            }
            else
            {
                if (m_BuffHintDic.ContainsKey(item.Key))
                {
                    await GTools.UnLoadWorldBuffHintAsync(m_BuffHintDic[item.Key]);
                    m_BuffHintDic.Remove(item.Key);
                }
            }
        }
    }



    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 攻击 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
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

        return CurrentBlood;
    }
    public virtual int ChangeMagic(int f_Increment)
    {
        var value = CurrentMagic + f_Increment;

        CurrentMagic = Mathf.Clamp(value, 0, MaxMagic);

        return CurrentMagic;
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- buff 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    // buff 列表
    [SerializeField] protected Dictionary<ushort, BuffData> m_BuffDic = new();
    protected ListStack<ushort> m_AddBuffDic = new("", 5);
    protected ListStack<ushort> m_RemoveBuffDic = new("", 5);
    [SerializeField] protected List<ushort> m_ImmuneBuffList = new();
    protected Dictionary<uint, WorldBuffHint> m_BuffHintDic = new();


    // 添加 buff
    public void AddBuffAsync(ushort f_BuffID, ushort f_Level)
    {
        if (m_ImmuneBuffList.Contains(f_BuffID))
        {
            GTools.WorldMgr.DamageText("免疫", EDamageType.None, BeHitPoint);
        }
        else if (m_BuffDic.TryGetValue(f_BuffID, out var IBuffBase))
        {
            IBuffBase.AddAsync(f_Level);
        }
    }

    // 移除 buff
    public void RemoveBuffAsync<T>(T f_IBuffBase) where T : BuffData
    {

    }

    // 清除所有 buff
    public void ClearBuffsAsync<T>(T f_IBuffBase) where T : BuffData
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




















