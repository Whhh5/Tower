using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityTowerBaseData : WorldObjectBaseData
{
    protected EntityTowerBaseData(int f_index, int f_ChunkIndex) : base(f_index, f_ChunkIndex)
    {
    }

    public abstract ETowerType TowerType { get; }
    public override ELayer AttackLayerMask => ELayer.Enemy;
    public override EWorldObjectType ObjectType => EWorldObjectType.Construction;



    protected int m_CurSkillCount = 0;
    public virtual int SkillStageCount { get; } = 1;
    public int CurStage => m_CurSkillCount % SkillStageCount;
    protected WorldObjectBaseData m_AttackTarget = null;
    // ¹¥»÷¼ä¸ô
    protected abstract float AtkIntervalOriginal { get; }
    protected float m_AddAtkinterval = 0;
    public float AtkInterval => Mathf.Clamp(AtkIntervalOriginal + m_AddAtkinterval, 0.2f, 10f);
    private float m_LastAtkTime = 0;

    // Æô¶¯×´Ì¬
    public ETowerStatus CurTowerStatus = ETowerStatus.Stop;
    public void SetTowerStatus(ETowerStatus f_TowerStatus)
    {
        switch (f_TowerStatus)
        {
            case ETowerStatus.Start:
                SetPersonStatus(EPersonStatusType.Walk, EAnimatorStatus.Stop);
                break;
            case ETowerStatus.Stop:
                SetPersonStatus(EPersonStatusType.Idle, EAnimatorStatus.Stop);
                break;
            case ETowerStatus.Destriy:
                SetPersonStatus(EPersonStatusType.Die, EAnimatorStatus.Stop);
                break;
            default:
                break;
        }
        CurTowerStatus = f_TowerStatus;
    }
    // Update 
    public override bool IsUpdateEnable => true;
    public override void OnUpdate()
    {
        base.OnUpdate();

        switch (CurStatus)
        {
            case EPersonStatusType.None: // ¾²Ö¹¶¯»­
                break;
            case EPersonStatusType.Idle: // Í£Ö¹¶¯»­
                break;
            case EPersonStatusType.Walk: // Æô¶¯¶¯»­
                break;
            case EPersonStatusType.Attack:
                {
                    if (m_AttackTarget != null && GTools.UnityObjectIsActive(m_AttackTarget))
                    {
                        if (false)
                        {

                        }
                        else if (CurrentMagic == 1)
                        {
                            SetPersonStatus(EPersonStatusType.Skill);
                        }
                        else if ((Time.time - m_LastAtkTime) > AtkInterval)
                        {
                            m_LastAtkTime = Time.time;
                        }
                    }
                }
                break;
            case EPersonStatusType.Skill:
                break;
            case EPersonStatusType.Die:
                break;
            case EPersonStatusType.Control:
                break;
            default:
                break;
        }
    }
    public override void AnimatorCallback050()
    {
        base.AnimatorCallback050();
        switch (CurStatus)
        {
            case EPersonStatusType.Attack:
                AttackTarget();
                break;
            default:
                break;
        }
    }
    public override void AnimatorCallback100()
    {
        base.AnimatorCallback100();
        switch (CurStatus)
        {
            case EPersonStatusType.Idle:
                SetPersonStatus(EPersonStatusType.None);
                break;
            case EPersonStatusType.Walk:
                SetPersonStatus(EPersonStatusType.Attack);
                break;
            case EPersonStatusType.Skill:
                CurrentMagic = 0;
                m_CurSkillCount++;
                SetPersonStatus(EPersonStatusType.Attack);
                break;
            default:
                break;
        }
    }
    public override string GetCurrentAnimationName()
    {
        var curName = base.GetCurrentAnimationName();

        if (CurStatus == EPersonStatusType.Skill)
        {
            curName = $"{curName}_{CurStage + 1}";
        }
        return curName;
    }
}
public class EntityTowerBase : WorldObjectBase
{

}
