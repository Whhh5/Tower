using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Entity_Gain_AddAttackHarm1Data : EntityGainBaseData
{
    public override EGainType GainType => EGainType.AttackHarm1;

    public override EBuffView GainView => EBuffView.perpetual;

    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Gain_AddAttackHarm1;
    private Entity_Gain_AddAttackHarm1 EntityTarget => GetCom<Entity_Gain_AddAttackHarm1>();

    private float m_AddAttackHarm = 0.2f;

    public Color StartMainColor = Color.white;
    public Color MainColor = Color.white;
    private Color FromColor = new Color(1, 1, 1, 0.2f);
    private Color ToColor = new Color(1, 1, 1, 0.5f);
    private float MoveTime = 1.0f;
    private AnimationCurve m_AnimaCurve = new();
    public override void AfterLoad()
    {
        base.AfterLoad();

        m_AnimaCurve.AddKey(0, 0);
        m_AnimaCurve.AddKey(1, 1);
    }
    public override void StartExecute()
    {
        m_Recipient.ChangeHarm(m_AddAttackHarm);


        base.StartExecute();
    }
    public override void StopExecute()
    {
        m_Recipient.ChangeHarm(-m_AddAttackHarm);


        base.StopExecute();
    }
    public override void ExecuteContext(int f_CurProbability)
    {
        
    }
    public void SetMainColor(Color f_Color)
    {
        MainColor = f_Color;
        if (EntityTarget != null)
        {
            EntityTarget.SetMainColor();
        }
    }
    public override Vector3 GetPosiion()
    {
        return m_Recipient.WorldPosition;
    }
    private float m_CurMoveTime = 0.0f;
    public override bool IsUpdateEnable => true;
    public override EUpdateLevel UpdateLevel => EUpdateLevel.Level1;
    public override void OnUpdate()
    {
        base.OnUpdate();

        m_CurMoveTime += UpdateDelta;
        var curSlider = (m_CurMoveTime % MoveTime) / MoveTime;
        var curValue = m_AnimaCurve.Evaluate(1 - Mathf.Abs(curSlider * 2 - 1));
        var color = Color.Lerp(FromColor, ToColor, curValue);
        SetMainColor(color);
    }
}
public class Entity_Gain_AddAttackHarm1 : EntityGainBase
{
    private Entity_Gain_AddAttackHarm1Data EntityData => GetData<Entity_Gain_AddAttackHarm1Data>();
    [SerializeField]
    private SpriteRenderer m_MainSpriteRenderer = null;

    public void SetMainColor()
    {
        m_MainSpriteRenderer.color = EntityData.MainColor;
    }
}
