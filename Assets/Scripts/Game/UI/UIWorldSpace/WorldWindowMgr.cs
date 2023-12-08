using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using B1.UI;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class WorldWindowMgr : Singleton<WorldWindowMgr>
{

    [SerializeField]
    private Transform m_Root => GTools.UIWindowManager.GetUIRootAsync(EUIAppRoot.Scene);

    public override async void Awake()
    {
        base.Awake();

        //var obj = new GameObject("Hint Root");
        //var tran = obj.AddComponent<RectTransform>();
        //tran.SetParent(GTools.UIMgr.UICanvasRect);
        //tran.SetSiblingIndex(0);
        //tran.localPosition = Vector3.zero;
        //tran.localScale = Vector3.one;
        //tran.localRotation = Quaternion.Euler(Vector3.zero);
        //m_Root = tran;
    }


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 生命值显示 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public Dictionary<int, WorldUIEntityHintData> m_BloodData = new();
    public void UpdateBloodHint(WorldObjectBaseData f_Target)
    {
        if (!GTools.UnityObjectIsVaild(f_Target))
        {
            return;
        }
        var key = f_Target.LoadKey;
        if (!m_BloodData.TryGetValue(key, out var value))
        {
            value = new(key, f_Target);
            value.SetParent(m_Root);
            m_BloodData.Add(key, value);
            GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(value));
        }
        value.UpdateInfo();
    }
    public void RemoveBloodHint(WorldObjectBaseData f_Target)
    {
        var key = f_Target.LoadKey;

        if (m_BloodData.TryGetValue(key, out var value))
        {
            ILoadPrefabAsync.UnLoad(value);
            m_BloodData.Remove(key);
        }
    }

    public void UpdateMagicHint(WorldObjectBaseData f_Target)
    {
        var key = f_Target.LoadKey;

        if (!m_BloodData.TryGetValue(key, out var value))
        {
            value = new(key, f_Target);
            value.SetParent(m_Root);
            m_BloodData.Add(key, value);
            GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(value));
        }
        value.UpdateInfo();
    }

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 伤害飘字 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public void CreateDamageHint(float f_Value, Vector3 f_WorldPos)
    {
        var effect = new UIDamageHintItemData();
        effect.SetParent(m_Root);

        var fontSize = Mathf.FloorToInt(Mathf.Clamp01((Mathf.Abs(f_Value) - 20) / 20) * 20 + 20);
        effect.SetFontSize(fontSize);
        effect.SetTMPText($"{f_Value}");

        effect.SetPosition(f_WorldPos);
        effect.SetFontColor(f_Value > 0 ? Color.green : Color.red);

        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(effect));
    }


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 进度条 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public void CreateWorldSlider(WorldObjectBaseData f_Target, Func<float> f_Update, Func<bool> f_Condition)
    {
        var sliderData = new WorldUISliderInfoData();
        sliderData.Initialization(f_Target, f_Update, f_Condition);
        sliderData.SetParent(m_Root);
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 技能栏 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    private Dictionary<Entity_HeroBaseData, UISkillInfoData> m_HeroSkillPlaneList = new();
    public void CreateSkillPlane(Entity_HeroBaseData f_Target)
    {
        var skillData = new UISkillInfoData();
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(skillData));
        skillData.Initialization(f_Target);
        skillData.SetParent(m_Root);
        m_HeroSkillPlaneList.Add(f_Target, skillData);
    }
    public void DestroySkillPlane(Entity_HeroBaseData f_Target)
    {
        if (m_HeroSkillPlaneList.TryGetValue(f_Target, out var value))
        {
            ILoadPrefabAsync.UnLoad(value);
            m_HeroSkillPlaneList.Remove(f_Target);
        }
    }
    public void AddHeroSkill(HeroAddSKillEventData f_Target)
    {
        if (m_HeroSkillPlaneList.TryGetValue(f_Target.HeroData, out var value))
        {
            value.AddSkill(f_Target.SkillType);
        }
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 攻击特效 篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public void CreateAttackEffect(Vector3 f_WorldPosition, EAttackEffectType f_EffectType)
    {
        if (GTools.TableMgr.TryGetAttactEffectBaseData(f_EffectType, out var eff, f_WorldPosition))
        {
            GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(eff));
        }
    }

}
