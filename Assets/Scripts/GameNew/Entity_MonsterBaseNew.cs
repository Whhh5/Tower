using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public abstract class Entity_MonsterBaseNewData : WorldObjectBaseData
{
    public override EEntityType EntityType => EEntityType.Person;

    public override ELayer LayerMask => ELayer.Enemy;

    public override ELayer AttackLayerMask => ELayer.Player;

    public override EWorldObjectType ObjectType => EWorldObjectType.Preson;
    private new Entity_MonsterBaseNew EntityTarget => GetCom<Entity_MonsterBaseNew>();

    // 路径相关
    private string DGID_Move => $"{EDGWorldID.MonsterMove}_{LoadKey}";
    private ListStack<PathElementData> m_CurPathList = new();
    private int m_CurPathIndexTarget = -1;
    private bool m_IsMoveing = false;
    private float MoveTimeInterval => 1.0f / CurMoveSpeed;
    private float LastMoveTime = 0.0f;
    // 攻击检测相关
    protected WorldObjectBaseData m_CurAttackTarget = null;
    private bool m_IsAttacking = false;
    // 攻击相关
    protected virtual bool IsSkill => false;
    protected virtual Vector3 WeaponStartPosOffset => new Vector3(0.5f, 0, -2f);
    public Vector3 WeaponStartPos => WeaponStartPosOffset + WorldPosition;
    protected virtual Vector3 WeaponStartUpOffset => Vector3.zero;
    public Vector3 WeaponStartUp => WeaponStartUpOffset + Vector3.up;
    public Color WeaponStartColor => Color.white;
    public string DGID_Atk => $"{EDGWorldID.MonsterAttack}";
    public float m_LastAtkTime = 0;
    public float AtkInterval => 1.0f / CurAtkSpeed;
    public float MoveInterval => 1.0f / CurMoveSpeed;
    public bool Skilling = false;
    public bool Attacking = false;
    public Vector3 WeaponPos;
    public Vector3 WeaponUp;
    public Color WeaponColor;

    public override void AfterLoad()
    {
        base.AfterLoad();
        UpdateWorldPosition(WeaponStartPos);
        UpdateDirectionUp(WeaponStartUp);
        SetPersonStatus(EPersonStatusType.Idle);
    }
    public override void UnLoad()
    {
        base.UnLoad();
        DOTween.Kill(DGID_Move);
    }
    public override void Death()
    {
        base.Death();

        DOTween.To(() => 0.0f, slider =>
        {
            SetAllElementColorAlpha(1 - slider);

        }, 1.0f, 1.0f).OnComplete(() =>
            {
                ILoadPrefabAsync.UnLoad(this);
            });

    }
    public override bool IsUpdateEnable => true;
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!GetObjBehaviorStatus())
        {
            return;
        }

        switch (CurStatus)
        {
            case EPersonStatusType.None:
            case EPersonStatusType.Incubation:
            case EPersonStatusType.Entrance:
            case EPersonStatusType.Idle:
                if (false)
                {

                }
                else if (IsPassSkill())
                {
                    SetPersonStatus(EPersonStatusType.Skill);
                }
                else if (TryAttackDetection())
                {
                    Attacking = true;
                    SetPersonStatus(EPersonStatusType.Attack);
                }
                else if (IsWalk())
                {
                    m_IsMoveing = true;
                    SetPersonStatus(EPersonStatusType.Walk);
                }
                break;
            case EPersonStatusType.Walk:
                {
                    if (m_IsMoveing)
                    {
                        if (Time.time - LastMoveTime > MoveTimeInterval)
                        {
                            LastMoveTime = Time.time;
                            if (IsNextMove())
                            {
                                MoveBehavior();
                            }
                            else
                            {
                                m_IsMoveing = false;
                            }
                        }
                    }
                    else
                    {
                        SetPersonStatus(EPersonStatusType.Idle);
                    }
                }
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
                if (!Skilling)
                {
                    if (GTools.UnityObjectIsVaild(m_CurAttackTarget))
                    {
                        Skilling = true;
                        SkillBehavior();
                    }
                    else
                    {
                        SetPersonStatus(EPersonStatusType.Idle);
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
    }
    public override bool IsPassSkill()
    {
        return base.IsPassSkill() && IsSkill && GTools.UnityObjectIsVaild(m_CurAttackTarget);
    }
    public virtual void SkillBehavior()
    {
        Skilling = false;
        ResetMagic();
        SetPersonStatus(EPersonStatusType.Idle);
    }
    private bool IsAttack()
    {
        return GTools.UnityObjectIsVaild(m_CurAttackTarget) && !IsPassSkill();
    }
    public virtual void AttackBehavior()
    {

    }
    public bool IsWalk()
    {
        if (this.TryGetRandomNearTarget(out var target))
        {
            UpdateCurPath(target.CurrentIndex);
            return true;
        }
        else if (this.TryGetNearTarget(EWorldObjectType.Resource, out var target2))
        {
            if (!IsNextMove() || (m_CurPathIndexTarget != target2.CurrentIndex && m_CurPathList.Count > 0))
            {
                UpdateCurPath(target2.CurrentIndex);
            }
            return true;
        }
        return false;
    }
    private void UpdateCurPath(int f_Index)
    {
        if (this.TryGetPath(f_Index, out m_CurPathList))
        {
            m_CurPathList.TryPop(out var _);
            m_CurPathIndexTarget = f_Index;
        }
    }
    private bool IsNextMove()
    {
        if (!m_CurPathList.TryValue(out var pathNode))
        {
            return false;
        }
        if (!GTools.CreateMapNew.TryGetChunkData(pathNode.Index, out var chunkData))
        {
            return false;
        }
        if (!chunkData.IsPass())
        {
            return false;
        }
        return true;
    }
    private void MoveBehavior()
    {
        if (!m_CurPathList.TryPop(out var pathNode))
        {
            return;
        }
        if (!GTools.CreateMapNew.TryGetChunkData(pathNode.Index, out var chunkData))
        {
            return;
        }


        var moveTime = MoveInterval * 0.7f;
        var posZ = WorldPosition.z;
        var curPos = WorldPosition;
        var targetPos = chunkData.WorldPosition;

        this.MoveToChunk(chunkData.ChunkIndex);
        DOTween.To(() => 0.0f, slider =>
        {
            Vector3 pos = Vector2.Lerp(curPos, targetPos, slider);
            pos.z = posZ;
            SetPosition(pos);

        }, 1.0f, moveTime)
            .SetId(DGID_Move)
            .OnComplete(() =>
            {
                m_IsMoveing = false;
            });
    }
    private bool TryAttackDetection()
    {
        if (this.TryGetRandomNearTarget(out m_CurAttackTarget))
        {
            return true;
        }
        return false;
    }

    public void UpdateWorldPosition(Vector3? f_Pos = null)
    {
        var pos = f_Pos ?? WeaponPos;
        WeaponPos = pos;
        if (EntityTarget != null)
        {
            EntityTarget.UpdateWeaponPos();
        }
    }
    public void UpdateDirectionUp(Vector3? f_Up = null)
    {
        var up = f_Up ?? WeaponUp;
        WeaponUp = up;
        if (EntityTarget != null)
        {
            EntityTarget.UpdateWeaponUp();
        }
    }
}
public abstract class Entity_MonsterBaseNew : WorldObjectBase
{
    public Entity_MonsterBaseNewData MonsterBaseData => GetData<Entity_MonsterBaseNewData>();

    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        await base.OnStartAsync(f_Params);

        UpdateWeaponPos();
        UpdateWeaponUp();
    }

    [SerializeField]
    private Transform m_AtkWeapon = null;
    public Vector3 AtkWeaponPos => m_AtkWeapon.position;
    public Vector3 DirectionUp => m_AtkWeapon.up;
    public void UpdateWeaponPos()
    {
        m_AtkWeapon.position = MonsterBaseData.WeaponPos;
    }
    public void UpdateWeaponUp()
    {
        m_AtkWeapon.up = MonsterBaseData.WeaponUp;
    }
}
