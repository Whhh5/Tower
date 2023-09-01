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
    private RectTransform m_Root = null;

    public override void Awake()
    {
        base.Awake();

        var obj = new GameObject("Hint Root");
        var tran = obj.AddComponent<RectTransform>();
        tran.SetParent(GTools.UIMgr.UICanvasRect);
        tran.SetSiblingIndex(0);
        tran.localPosition = Vector3.zero;
        tran.localScale = Vector3.one;
        tran.localRotation = Quaternion.Euler(Vector3.zero);
        m_Root = tran;
    }


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- ����ֵ��ʾ ƪ
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public Dictionary<int, WorldUIEntityHintData> m_BloodData = new();
    public void UpdateBloodHint(WorldObjectBaseData f_Target)
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
    //                                catalogue -- ������ ƪ
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public void CreateWorldSlider(WorldObjectBaseData f_Target, Func<float> f_Update, Func<bool> f_Condition)
    {
        var sliderData = new UISliderInfoData();
        sliderData.Initialization(f_Target, f_Update, f_Condition);
        sliderData.SetParent(m_Root);
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- ������Ч ƪ
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
