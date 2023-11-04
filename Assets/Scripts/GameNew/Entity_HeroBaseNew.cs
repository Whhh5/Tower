using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public abstract class Entity_HeroBaseNewData : WorldObjectBaseData
{
    public abstract EHeroCardType HeroType { get; }
    public override EEntityType EntityType => EEntityType.Person;

    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;

    public override EWorldObjectType ObjectType => EWorldObjectType.Preson;

    private new Entity_HeroBaseNew EntityTarget => GetCom<Entity_HeroBaseNew>();

    // 攻击相关参数
    protected virtual Vector3 WeaponStartPosOffset => new Vector3(-0.5f, 0, -2f);
    public Vector3 WeaponStartPos => WeaponStartPosOffset + WorldPosition;
    protected virtual Vector3 WeaponStartUpOffset => Vector3.zero;
    public Vector3 WeaponStartUp => WeaponStartUpOffset + Vector3.up;
    public Color WeaponStartColor => Color.white;
    protected virtual Vector3 WeaponStartScaleOffset => new Vector3(1, 1, 1);
    public Vector3 WeaponStartScale => WeaponStartScaleOffset;
    protected virtual string DGID_Atk => $"{EDGWorldID.HeroAttack}_{LoadKey}";
    protected virtual string DGID_Skill => $"{EDGWorldID.HeroSkill}_{LoadKey}";
    protected virtual string DGID_Move => $"{EDGWorldID.HeroMove}_{LoadKey}";
    protected WorldObjectBaseData m_CurAttackTarget = null;

    public override bool IsUpdateEnable => true;
    // 攻击相关
    public float m_LastAtkTime = 0;
    public float AtkInterval => 1.0f / CurAtkSpeed * 0.8f;
    public float AtkBehavior => AtkInterval * 0.8f;
    public float MoveInterval => 1.0f / CurMoveSpeed;
    public virtual EAssetKey SkillElement => EAssetKey.Entity_Default_SkillElement;
    public bool Skilling = false;
    public bool Attacking = false;

    public override void AfterLoad()
    {
        base.AfterLoad();

        UpdateWeaponPosition(WeaponStartPos);
        UpdateWeaponDirectionUp(WeaponStartUp);
        SetWeaponColor(WeaponStartColor);
        SetPersonStatus(EPersonStatusType.Idle);
        SetWeaponLocalSacle(WeaponStartScale);
    }
    public override void Death()
    {
        base.Death();
        GTools.FormationMgr.FormationRemove(this);
        GTools.HeroCardPoolMgr.RecycleGroupCrad(HeroType);
        DOTween.To(() => 0.0f, slider =>
        {
            SetAllElementColorAlpha(1 - slider);

        }, 1.0f, 1.0f)
            .OnComplete(() =>
            {
                ILoadPrefabAsync.UnLoad(this);
            });
    }
    public override void OnUpdate()
    {
        base.OnUpdate();


        switch (CurStatus)
        {
            case EPersonStatusType.None:
            case EPersonStatusType.Incubation:
            case EPersonStatusType.Entrance:
            case EPersonStatusType.Idle:
                {
                    // 攻击检测
                    if (false)
                    {

                    }
                    else if (IsPassSkill())
                    {
                        SetPersonStatus(EPersonStatusType.Skill);
                    }
                    else if (Time.time - m_LastAtkTime > AtkInterval && AttackDetection())
                    {
                        m_LastAtkTime = Time.time + AtkInterval * 0.5f;
                        Attacking = true;
                        SetPersonStatus(EPersonStatusType.Attack);
                    }
                }
                break;
            case EPersonStatusType.Walk:
                break;
            case EPersonStatusType.Attack:
                {
                    if (Attacking)
                    {
                        if (Time.time - m_LastAtkTime > AtkInterval)
                        {
                            m_LastAtkTime = Time.time;
                            if (IsAttack())
                            {
                                AttackBehavior();
                            }
                            else
                            {
                                Attacking = false;
                            }
                        }
                    }
                    else
                    {
                        SetPersonStatus(EPersonStatusType.Idle);
                    }
                }
                break;
            case EPersonStatusType.Skill:
                {
                    if (!Skilling)
                    {
                        if (GTools.UnityObjectIsVaild(m_CurAttackTarget) || this.TryGetRandomNearTarget(out m_CurAttackTarget))
                        {
                            Skilling = true;
                            SkillBehavior();
                        }
                        else
                        {
                            SetPersonStatus(EPersonStatusType.Idle);
                        }
                    }
                }
                break;
            case EPersonStatusType.Die:
                break;
            case EPersonStatusType.Control:
                break;
            default:
                break;
        }





        // 划线检测
        m_GizomTargets.Clear();
        if (GTools.UnityObjectIsVaild(m_CurAttackTarget))
        {
            m_GizomTargets.Add(m_CurAttackTarget);
        }


    }
    private bool IsAttack()
    {
        return GTools.UnityObjectIsVaild(m_CurAttackTarget) && !IsPassSkill();
    }
    private bool AttackDetection()
    {
        if (!GTools.CreateMapNew.TryGetRangeChunkByIndex(CurrentIndex, out var listIndex, null, false, CurAtkRange))
        {
            return false;
        }

        if (!GTools.UnityObjectIsVaild(m_CurAttackTarget) || !listIndex.Contains(m_CurAttackTarget.CurrentIndex))
        {
            if (!this.TryGetRandomNearTarget(out m_CurAttackTarget))
            {
                return false;
            }
        }
        return true;
    }
    public virtual void AttackBehavior()
    {

    }
    public virtual void SkillBehavior()
    {
        Skilling = false;
        ResetMagic();
        SetPersonStatus(EPersonStatusType.Idle);
    }



    public Vector3 WeaponPos;
    public Vector3 WeaponUp;
    public Vector3 WeaponScale;
    public Color WeaponColor;
    public void UpdateWeaponPosition(Vector3? f_Pos = null)
    {
        var pos = f_Pos ?? WeaponPos;
        WeaponPos = pos;
        if (EntityTarget != null)
        {
            EntityTarget.UpdateWeaponPos();
        }
    }
    public Vector3 GetWeaponPosition()
    {
        if (EntityTarget != null)
        {
            return EntityTarget.GetWeaponPosition();
        }
        return WeaponStartPos;
    }
    public Vector3 GetWeaponUp()
    {
        if (EntityTarget != null)
        {
            return EntityTarget.GetWeaponUp();
        }
        return WeaponStartUp;
    }
    public Vector3 GetWeaponLocalSacle()
    {
        if (EntityTarget != null)
        {
            return EntityTarget.GetWeaponLocalScale();
        }
        return WeaponScale;
    }
    public void SetWeaponLocalSacle(Vector3 f_LocalScale)
    {
        WeaponScale = f_LocalScale;
        if (EntityTarget != null)
        {
            EntityTarget.SetWeaponLocalScale();
        }
    }
    public float GetWeaponRotation()
    {
        if (EntityTarget != null)
        {
            return EntityTarget.GetWeaponRotation();
        }
        return 0;
    }
    public void UpdateWeaponDirectionUp(Vector3? f_Up = null)
    {
        var up = f_Up ?? WeaponUp;
        WeaponUp = up;
        if (EntityTarget != null)
        {
            EntityTarget.UpdateWeaponUp();
        }
    }
    public void SetWeaponColor(Color? f_Color = null)
    {
        var color = f_Color ?? WeaponColor;
        WeaponColor = color;
        if (EntityTarget != null)
        {
            EntityTarget.SetWeaponColor();
        }
    }
    public override void OnMouseEnterRange()
    {
        this.ShowAttackRange(Color.green);
    }
    public override void OnMouseExitRange()
    {
        this.HideAttackRange();
    }

    public virtual TElement GetWeaponElement<TElement>()
        where TElement : Entity_SkillElementBaseData, new()
    {
        var ele = new TElement();
        return ele;
    }

    // 划线检测
    private List<UnityObjectData> m_GizomTargets = new();
    public void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //UnityEditor.Handles.color = Color.green;
        //UnityEditor.Handles.lighting = true;
        foreach (var item in m_GizomTargets)
        {
            //UnityEditor.Handles.DrawLine(WorldPosition, item.WorldPosition);
            //Gizmos.DrawLine(WorldPosition, item.WorldPosition);
            Debug.DrawLine(PrefabTarget.Position, item.WorldPosition, Color.red, 3.0f);
        }
    }
}
public abstract class Entity_HeroBaseNew : WorldObjectBase
{
    protected Entity_HeroBaseNewData HeroBaseData => GetData<Entity_HeroBaseNewData>();

    public override void OnUpdate()
    {
        base.OnUpdate();

    }
    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        await base.OnStartAsync(f_Params);

        InitData();
        UpdateWeaponPos();
        UpdateWeaponUp();
        SetWeaponLocalScale();
    }

    [SerializeField]
    private Transform m_AtkWeapon = null;
    public Vector3 GetWeaponPosition()
    {
        return m_AtkWeapon.position;
    }
    public Vector3 GetWeaponUp()
    {
        return m_AtkWeapon.up;
    }
    public float GetWeaponRotation()
    {
        return m_AtkWeapon.localEulerAngles.z;
    }
    public Vector3 GetWeaponLocalScale()
    {
        return m_AtkWeapon.localScale;
    }
    public void SetWeaponLocalScale()
    {
        m_AtkWeapon.localScale = HeroBaseData.WeaponScale;
    }
    public void UpdateWeaponPos()
    {
        m_AtkWeapon.position = HeroBaseData.WeaponPos;
    }
    public void UpdateWeaponUp()
    {
        m_AtkWeapon.up = HeroBaseData.WeaponUp;
    }
    public void InitData()
    {

    }
    public void SetWeaponColor()
    {
        var coms = m_AtkWeapon.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer item in coms)
        {
            item.color = HeroBaseData.WeaponColor;
        }
    }

    private void OnDrawGizmos()
    {
        if (HeroBaseData == null)
        {
            return;
        }
        HeroBaseData.OnDrawGizmos();
    }
}
