using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public class PlayerData
{

}
public class PlayerMgr : Singleton<PlayerMgr>, IUpdateBase
{
    public float UpdateDelta { get; set; }
    public float LasteUpdateTime { get; set; }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 生命周期篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public override void Awake()
    {
        base.Awake();
        InitPlayerData();
    }
    public override void Start()
    {
        base.Start();

        InitGold();
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 玩家属性篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public EPlayerLevel Level { get; private set; }
    public void InitPlayerData()
    {
        Level = EPlayerLevel.Level1;
    }




    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 金币篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public UIGoldWindow UIGoldWindow = null;
    public void InitGold()
    {
        UIGoldWindow = GameObject.FindObjectOfType<UIGoldWindow>();
        SetGoldCount(0);

        GTools.LifecycleMgr.AddUpdate(this);
    }
    public void SetGoldCount(int f_Count)
    {
        CurGoldNum = f_Count;
        UpdateGoldCount();
    }
    /// <summary>
    /// 增加金币
    /// </summary>
    /// <param name="f_Increment"></param>
    public void Increases(int f_Increment)
    {
        var value = Mathf.Max(f_Increment, 0);

        var curGoldCount = CurGoldNum += value;
        SetGoldCount(curGoldCount);
    }
    /// <summary>
    /// 减少金币
    /// </summary>
    /// <param name="f_Increment"></param>
    /// <returns></returns>
    public void Reduce(int f_Increment)
    {
        var value = Mathf.Max(f_Increment, 0);

        var curGoldCount = Mathf.Max(CurGoldNum - value, 0);
        SetGoldCount(curGoldCount);
    }
    /// <summary>
    /// 尝试花费金币
    /// </summary>
    /// <param name="f_Increment"></param>
    /// <returns></returns>
    public bool TryExpenditure(int f_Increment)
    {
        if (CurGoldNum >= f_Increment)
        {
            Reduce(f_Increment);
            return true;
        }
        return false;
    }
    private void UpdateGoldCount()
    {
        if (UIGoldWindow != null)
        {
            UIGoldWindow.UpdateGoldCount();
        }
    }




    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- Update 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public int CurGoldNum { get; private set; }
    public int UpdateLevelID { get; set; }
    public EUpdateLevel UpdateLevel => EUpdateLevel.Level1;
    private float m_LastIncreasesTime = 0;
    private float m_IncreasesInterval = 1;
    public void OnUpdate()
    {
        if (Time.time - m_LastIncreasesTime > m_IncreasesInterval)
        {
            Increases(1);
            m_LastIncreasesTime = Time.time;
        }
    }
}
