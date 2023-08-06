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
    //                                catalogue -- �������� ƪ
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
    //                                catalogue -- ���� ƪ
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    // ������ 0 - 1
    public float CriticalChance { get; private set; } = 0.2f;
    // ��������, �൱�ڹ����Ķ��ٱ� 1 - n
    public float CriticalMultiple { get; private set; } = 2;

    /// <summary>
    /// ��ȡ��ǰ�ɹ���״̬
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
    //                                catalogue -- Ѫ������ ƪ
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
    //                                catalogue -- buff ƪ
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    // buff �б�
    [SerializeField] protected Dictionary<ushort, BuffData> m_BuffDic = new();
    protected ListStack<ushort> m_AddBuffDic = new("", 5);
    protected ListStack<ushort> m_RemoveBuffDic = new("", 5);
    [SerializeField] protected List<ushort> m_ImmuneBuffList = new();
    protected Dictionary<uint, WorldBuffHint> m_BuffHintDic = new();


    // ��� buff
    public void AddBuffAsync(ushort f_BuffID, ushort f_Level)
    {
        if (m_ImmuneBuffList.Contains(f_BuffID))
        {
            GTools.WorldMgr.DamageText("����", EDamageType.None, BeHitPoint);
        }
        else if (m_BuffDic.TryGetValue(f_BuffID, out var IBuffBase))
        {
            IBuffBase.AddAsync(f_Level);
        }
    }

    // �Ƴ� buff
    public void RemoveBuffAsync<T>(T f_IBuffBase) where T : BuffData
    {

    }

    // ������� buff
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




















