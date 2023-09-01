using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using Cysharp.Threading.Tasks;

public class LifecycleMgr : MonoSingleton<LifecycleMgr>
{
    public int UpdateKey = int.MinValue;
    public float CurFrameRate;
    private Dictionary<EUpdateLevel, UpdateData> m_DicUppdate = new();




    protected override void Awake()
    {
        base.Awake();


        for (byte i = 0; i < (byte)EUpdateLevel.EnumCount; i++)
        {
            var level = (EUpdateLevel)i;
            m_DicUppdate.Add(level, new(level));
        }
    }

    public void AddUpdate(IUpdateBase f_Target)
    {
        if (!m_DicUppdate[f_Target.UpdateLevel].Contains(f_Target))
        {
            f_Target.UpdateLevelID = UpdateKey++;
            m_DicUppdate[f_Target.UpdateLevel].Add(f_Target);
        }
    }
    public void RemoveUpdate(IUpdateBase f_Target)
    {
        m_DicUppdate[f_Target.UpdateLevel].Remove(f_Target);
    }




    private void Update()
    {
        CurFrameRate = 1 / (Time.deltaTime * Application.targetFrameRate);


        foreach (var item in m_DicUppdate)
        {
            item.Value.Execute(Mathf.Clamp01(CurFrameRate) * 100);
        }

    }
}

public class UpdateData
{
    private EUpdateLevel m_Level;
    public UpdateData(EUpdateLevel f_Level)
    {
        m_Level = f_Level;
    }
    // 当前更新的索引
    public int CurIndex = 0;
    // 当前帧最大可更新的数量
    public int MaxCount = 0;
    // 当前帧实际更新的数量
    public int CurCount = 0;
    // 当前帧更新完成时最后一个索引
    public int EndIndex = 0;
    // 当前根据游戏性能更新的百分比
    public float Percentage = 0;
    // 当前 Update 的数量
    private int Count => m_DicUppdate.Count;
    // Update 等级映射
    private (ushort tMin, ushort tMax, ushort tCount) LevelMap => GTools.LevelMap[m_Level];
    // 当前等级所有的 Update
    Dictionary<int, IUpdateBase> m_DicUppdate = new();
    // 需要添加的 Update
    ListStack<int> m_RemoveList = new("Update Remove List", 10);
    // 当前删除的 Update
    ListStack<IUpdateBase> m_AddList = new("Update Add List", 10);


    // 需要更新 update 的缩影区间
    int[] m_IndexArr = new int[4];
    // For 循环的索引
    int m_ForI = 0;
    public void Add(IUpdateBase f_Target)
    {
        m_AddList.Push(f_Target);
    }
    public bool Contains(IUpdateBase f_Target)
    {
        return m_AddList.Contains(f_Target);
    }
    public void Remove(IUpdateBase f_Target)
    {
        m_RemoveList.Push(f_Target.UpdateLevelID);
    }
    public void Execute(float f_CurFrameRate)
    {
        while (m_RemoveList.TryPop(out var value))
        {
            m_DicUppdate.Remove(value);
        }
        while (m_AddList.TryPop(out var value))
        {
            m_DicUppdate.Add(value.UpdateLevelID, value);
        }
        if (int.Equals(Count, 0)) return;
        CurIndex = Mathf.Clamp(CurIndex, 0, Count);
        Percentage = Mathf.Clamp01(f_CurFrameRate - LevelMap.tMin * GTools.MaxFrameRate) / ((LevelMap.tMax - LevelMap.tMin) * GTools.MaxFrameRate);

        // 当前帧最大可以执行的 update 数量
        MaxCount = (int)(Percentage * LevelMap.tCount);

        CurCount = Mathf.Min(MaxCount, Count);

        EndIndex = (CurIndex + CurCount) % Count;


        m_IndexArr[1] = EndIndex;
        m_IndexArr[2] = CurIndex;

        if (CurIndex < EndIndex)
        {
            m_IndexArr[0] = CurIndex;
            m_IndexArr[3] = EndIndex;
        }
        else
        {
            m_IndexArr[0] = 0;
            m_IndexArr[3] = Count;
        }



        m_ForI = 0;
        foreach (var item in m_DicUppdate)
        {
            if ((m_ForI >= m_IndexArr[0] && m_ForI < m_IndexArr[1]) || (m_ForI >= m_IndexArr[2] && m_ForI < m_IndexArr[3]))
            {
                if (item.Value.LasteUpdateTime == 0)
                {
                    item.Value.LasteUpdateTime = GTools.CurTime;
                }
                item.Value.UpdateDelta = GTools.CurTime - item.Value.LasteUpdateTime;
                item.Value.LasteUpdateTime = GTools.CurTime;
                item.Value.OnUpdate();
            }
            m_ForI++;
        }
        CurIndex = EndIndex;
    }
}