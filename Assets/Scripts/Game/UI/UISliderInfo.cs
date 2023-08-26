using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliderInfoData : UnityObjectData
{
    public UISliderInfoData() : base(0)
    {
    }

    private WorldObjectBaseData m_TargetEntity = null;
    private Func<float> m_UpdateValue = null;
    private Func<bool> m_Condition = null;
    public void Initialization(WorldObjectBaseData f_Target, Func<float> f_Update, Func<bool> FCondition)
    {
        m_TargetEntity = f_Target;
        m_UpdateValue = f_Update;
        m_Condition = FCondition;
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(this));
    }
    public override AssetKey AssetPrefabID => AssetKey.UISliderInfo;

    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    public UISliderInfo UISliderInfo => GetCom<UISliderInfo>();
    public override bool IsUpdateEnable => true;
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (GTools.UnityObjectIsActive(m_TargetEntity) && m_Condition.Invoke())
        {
            var value = m_UpdateValue.Invoke();
            UpdateSlider(value);

            var uguiPos = IUIUtil.GetUGUIPosByWorld(m_TargetEntity.PointUp - new Vector3(0, 0.2f, 0), true);
            var worldPos = IUIUtil.GetWorldPosBuyUGUIPos(uguiPos);
            SetPosition(worldPos);
        }
        else
        {
            ILoadPrefabAsync.UnLoad(this);
        }

    }
    public override void AfterLoad()
    {
        base.AfterLoad();
        UpdateSlider();
    }
    private float m_SliderValue = 0;
    private void UpdateSlider(float? f_Value = null)
    {
        var value = f_Value ?? m_SliderValue;
        m_SliderValue = value;
        if (UISliderInfo != null)
        {
            UISliderInfo.UpdateSlider(value);
        }
    }
}
public class UISliderInfo : ObjectPoolBase
{

    [SerializeField]
    private Image m_SliderImg = null;
    public void UpdateSlider(float f_Value)
    {
        m_SliderImg.fillAmount = f_Value;
    }
}
