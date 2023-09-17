using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;



public class HeroCradFetterInfo
{
    public EHeroFetterType Type;
    public int Count;
    public HeroFetterInfo CradInfo;
}

public class HeroFetterInfo_Electricity : HeroFetterInfo
{
    public override EHeroFetterType HeroFetterType => EHeroFetterType.Electricity;
    public override int[] LevelStage => new int[3] { 2, 4, 6 };

    public override void OnChangeStageClick(int f_ToStage)
    {
        Log($"HeroFetterInfo_Electricity fetter stage change {CurStage} => {f_ToStage}");
    }
}
public class HeroFetterMgr : Singleton<HeroFetterMgr>
{
    private Dictionary<EHeroFetterType, HeroFetterInfo> m_FetterInfo = new()
    {
        {
            EHeroFetterType.Electricity,
            new HeroFetterInfo_Electricity()
        },
    };

    // µ±Ç°Íæ¼Òî¿°í
    private Dictionary<EHeroFetterType, HeroCradFetterInfo> m_HeroFetter = new();

    public void Initialization()
    {
    }

    private void AddHeroFetter(EHeroFetterType f_FetterType)
    {
        if (m_FetterInfo.TryGetValue(f_FetterType, out var value))
        {
            value.ChangeCount(1);
        }
    }
}