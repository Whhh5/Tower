using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;
public enum ESkillIndex
{
    None,
    Skill_1,
    Skill_2,
    Skill_3,
    Skill_4,
    Skill_5,
}
public enum ESkillStage2
{
    Stage1,
    Stage2,
    EnumCount,
}
public enum ESkillStage3
{
    Stage1,
    Stage2,
    Stage3,
    EnumCount,
}
public enum ESkillStage4
{
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    EnumCount,
}
public enum ESkillStage5
{
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    Stage5,
    EnumCount,
}
public abstract class Entity_HeroBaseData : WorldObjectBaseData
{
    protected enum EMoveStatus
    {
        Play,
        Pause,
        Stop,
    }
    public Entity_HeroBaseData(int f_index, int f_ChunkIndex, EHeroCradStarLevel f_HeroStarLvevl) : base(f_index, f_ChunkIndex)
    {

    }

    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;
    public abstract EHeroCardType HeroCradType { get; }
    public sealed override EWorldObjectType ObjectType => EWorldObjectType.Preson;
    public Entity_HeroBase HeroData => GetCom<Entity_HeroBase>();

    public Vector3 AttackPoint => HeroData.m_AttackPoint != null ? HeroData.m_AttackPoint.position : WorldPosition;

    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 生命周期篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public override bool IsUpdateEnable => true;

    public override void OnUpdate()
    {
        base.OnUpdate();

        switch (CurStatus)
        {
            case EPersonStatusType.None:
                break;
            case EPersonStatusType.Incubation:
                break;
            case EPersonStatusType.Entrance:
                break;
            case EPersonStatusType.Idle:
                {
                    if (false)
                    {

                    }
                    else if (TryGetNextPoint(out var pathPoint))
                    {
                        WorldMapMgr.Ins.MoveChunkElement(this, pathPoint.ChunkIndex);
                        m_MoveToTarget = pathPoint.WorldPos;
                        SetPersonStatus(EPersonStatusType.Walk);
                    }
                    else if (TryDetectionAttack(out var target))
                    {
                        CurAttackTarget = target;
                        SetPersonStatus(EPersonStatusType.Attack);
                    }
                }
                break;
            case EPersonStatusType.Walk:
                {
                    var distance = Vector3.Magnitude(m_MoveToTarget - WorldPosition);
                    if (distance > 0.001f)
                    {
                        var value = Vector3.MoveTowards(WorldPosition, m_MoveToTarget, CurMoveSpeed * Time.deltaTime);
                        var forward = Vector3.MoveTowards(Forward, value - WorldPosition, CurMoveSpeed * Time.deltaTime * 100);
                        SetForward(forward);
                        SetPosition(value);
                    }
                    else
                    {
                        SetPersonStatus(EPersonStatusType.Idle);
                    }
                }
                break;
            case EPersonStatusType.Attack:
                {
                    if (MagicPercent >= 1)
                    {
                        SetPersonStatus(EPersonStatusType.Skill);
                        var value = Vector3.MoveTowards(Forward, CurAttackTarget.WorldPosition - WorldPosition, Time.deltaTime * 10);
                        SetForward(value);
                    }
                    if (!GTools.UnityObjectIsActive(CurAttackTarget)
                        || Vector3.Distance(CurAttackTarget.WorldPosition, WorldPosition) > AtkRange * 2)
                    {
                        SetPersonStatus(EPersonStatusType.Idle);
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
    public override void Resurgence()
    {
        base.Resurgence();

        WorldWindowMgr.Ins.CreateSkillPlane(this);
    }
    public override void Death()
    {
        base.Death();
        GTools.WorldWindowMgr.DestroySkillPlane(this);
        GTools.HeroMgr.DestroyHero(this);
    }


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 路径篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    protected EMoveStatus m_CurMoveStatus = EMoveStatus.Stop;
    private ListStack<PathElementData> m_CurPathList = new("");
    private Vector3 m_MoveToTarget = Vector3.zero;
    public void SetPath(ListStack<PathElementData> f_CurPath)
    {
        m_CurPathList = f_CurPath;
        SetPersonStatus(EPersonStatusType.Idle);
    }
    public void Play()
    {

        m_CurMoveStatus = EMoveStatus.Play;
    }
    public void Pause()
    {

        m_CurMoveStatus = EMoveStatus.Pause;
    }
    public void Stop()
    {

        m_CurMoveStatus = EMoveStatus.Stop;
    }
    public bool TryGetNextPoint(out PathElementData f_PathPoint)
    {
        return m_CurPathList.TryPop(out f_PathPoint);
    }



    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 攻击篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public virtual int AtkRange { get; } = 1;
    public virtual float AtkSpeed { get; } = 1.0f;
    public WorldObjectBaseData CurAttackTarget = null;
    public bool TryDetectionAttack(out WorldObjectBaseData f_AttackTarget)
    {
        f_AttackTarget = null;
        if (WorldMapMgr.Ins.TryGetRangeChunkByIndex(CurrentIndex, out var list, AttackTargetCondition, false, AtkRange))
        {
            var minDis = float.MaxValue;
            foreach (var item in list.GetEnumerator())
            {
                if (WorldMapMgr.Ins.TryGetChunkData(item.Value, out var chunkData))
                {
                    if (chunkData.GetWorldObjectByLayer(AttackLayerMask, out var targets))
                    {
                        foreach (var target in targets)
                        {
                            var dis = Vector3.Magnitude(target.Value.WorldPosition - WorldPosition);
                            if (GTools.UnityObjectIsActive(target.Value) && minDis > dis)
                            {
                                if (target.Value is WorldObjectBaseData worldObj)
                                {
                                    f_AttackTarget = worldObj;
                                    minDis = dis;
                                }
                            }
                        }
                    }
                }
            }
        }
        return f_AttackTarget != null;
    }
    public bool AttackTargetCondition(int f_Index)
    {
        if (WorldMapMgr.Ins.TryGetChunkData(f_Index, out var chunkData))
        {
            return chunkData.IsAlreadyLayer(AttackLayerMask);
        }
        return false;
    }


    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 技能篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public class SkillInfo
    {
        public EPersonSkillType SkillType;
        public PersonSkillBaseData SkillData;
        public SkillInfo NextStageSkills;
    }
    protected SkillInfo SkillLink = null;
    protected int CurSkillCount => GetCurSkillCount();
    public int GetCurSkillCount()
    {
        var count = 0;
        var nextSkill = SkillLink;
        while (nextSkill != null)
        {
            count++;
            nextSkill = nextSkill.NextStageSkills;
        }
        return count;
    }
    public SkillInfo GetSkill(ESkillIndex f_SkillIndex)
    {
        var tempSkillLink = SkillLink;
        for (int i = 0; i < (int)f_SkillIndex - 1; i++)
        {
            if (tempSkillLink.NextStageSkills != null)
            {
                tempSkillLink = tempSkillLink.NextStageSkills;
            }
        }
        return tempSkillLink;
    }
    public List<EPersonSkillType> GetCurSkillLink()
    {
        List<EPersonSkillType> result = new();
        var nextSkill = SkillLink;
        while (nextSkill != null)
        {
            result.Add(nextSkill.SkillType);
            nextSkill = nextSkill.NextStageSkills;
        }
        return result;
    }
    public bool TryGetCurEndSkill(out EPersonSkillType f_Skill)
    {
        f_Skill = EPersonSkillType.None;
        var skillLink = SkillLink;
        if (skillLink != null)
        {
            while (skillLink.NextStageSkills != null)
            {
                skillLink = skillLink.NextStageSkills;
            }
            f_Skill = skillLink.SkillType;
        }
        return f_Skill != EPersonSkillType.None;
    }
    public bool TryGetCurCanSkillCount(out int f_Count)
    {
        f_Count = -1;
        if (GTools.TableMgr.TryGetHeroCradInfo(HeroCradType, out var heroInfo))
        {
            var skillLink = heroInfo.SkillLinkInfos.SkillLink;
            var curSkillLink = GetCurSkillLink();
            if (curSkillLink.Count == 0)
            {
                f_Count = heroInfo.SkillLinkInfos.Count;
            }
            else
            {
                var loopIndex = -1;
                if (MainLoop())
                {
                    var nestCount = NextSkillLink(skillLink, 0);
                    f_Count = heroInfo.SkillLinkInfos.Count - nestCount - curSkillLink.Count;
                }
                bool MainLoop()
                {
                    if (++loopIndex >= curSkillLink.Count)
                    {
                        return true;
                    }
                    var nextSkill = curSkillLink[loopIndex];

                    foreach (var skillType in skillLink)
                    {
                        if (skillType.SkillType != nextSkill)
                        {
                            continue;
                        }
                        skillLink = skillType.NextStageSkills;
                        return true;
                    }
                    return false;
                }

                int NextSkillLink(List<SkillLink> targetList, int curNum)
                {
                    int maxNum = ++curNum;
                    foreach (var item in targetList)
                    {
                        if (item.NextStageSkills != null)
                        {
                            var nextNum = NextSkillLink(item.NextStageSkills, curNum);
                            maxNum = Mathf.Max(maxNum, nextNum);
                        }
                    }
                    return maxNum;
                }
            }
        }
        return f_Count > -1;
    }
    public void AddNextSkill(EPersonSkillType f_SkillType)
    {
        var tempSkillLink = SkillLink;
        if(GTools.TableMgr.TryGetPersonSkillData(f_SkillType, out var skilldata))
        {
            SkillInfo skillInfo = new()
            {
                NextStageSkills = null,
                SkillData = skilldata,
                SkillType = f_SkillType,
            };
            skilldata.Initialization(this);
            if (tempSkillLink == null)
            {
                SkillLink = skillInfo;
            }
            else
            {
                while (tempSkillLink.NextStageSkills != null)
                {
                    tempSkillLink = tempSkillLink.NextStageSkills;
                    continue;
                }
                tempSkillLink.NextStageSkills = skillInfo;
            }
            UniTask.Void(() => HeroData.OverrideAnima((ESkillIndex)CurSkillCount, f_SkillType));
        }
    }
    //--
    //===============================----------------------========================================
    //-----------------------------                          --------------------------------------
    //                                catalogue -- 技能动画行为篇
    //-----------------------------                          --------------------------------------
    //===============================----------------------========================================
    //--
    public int m_CurSkillCount = 0;
    public virtual int SkillStageCount { get; } = 1;
    public int CurStage
    {
        get
        {
            var skillCount = Mathf.Min(SkillStageCount, CurSkillCount);
            return skillCount > 0 ? m_CurSkillCount % skillCount : 0;
        }
    }
    public override void AnimatorCallback100()
    {
        base.AnimatorCallback100();
        switch (CurStatus)
        {
            case EPersonStatusType.Entrance:
                {
                    SetPersonStatus(EPersonStatusType.Idle);
                }
                break;
            case EPersonStatusType.Skill:
                {
                    CurrentMagic = 0;
                    m_CurSkillCount++;
                    SetPersonStatus(EPersonStatusType.Idle);
                }
                break;
            case EPersonStatusType.Die:
                {
                    ILoadPrefabAsync.UnLoad(this);
                }
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
            curName = $"{curName}_{CurStage}";
        }
        return curName;
    }

    public override void AttackTarget()
    {
        base.AttackTarget();

        GTools.WorldWindowMgr.CreateAttackEffect(CurAttackTarget.CentralPoint, EAttackEffectType.Default1);
    }

}
public abstract class Entity_HeroBase : WorldObjectBase
{
    private Entity_HeroBaseData DataEntity => GetData<Entity_HeroBaseData>();
    public Transform m_AttackPoint = null;
    private AnimatorOverrideController m_AnimOverCon;
    private Dictionary<string, string> m_StateToAssetName = new()
    {
        {
            "Skill_1",
            "Skill_1"
        },
        {
            "Skill_2",
            "Skill_2"
        },
        {
            "Skill_3",
            "Skill_3"
        },
        {
            "Skill_4",
            "Skill_5"
        },
    };

    private void OnMouseDown()
    {
        MoveCardMgr.Ins.SetCurSelectHero(DataEntity);
    }

    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();

        if (m_AnimOverCon == null)
        {
            m_AnimOverCon = new();
        }
        var con = CurAnim.runtimeAnimatorController;
        m_AnimOverCon.runtimeAnimatorController = CurAnim.runtimeAnimatorController;
        CurAnim.runtimeAnimatorController = m_AnimOverCon;

        //var anima = con as AnimatorController;
        //AnimatorStateMachine stateMachine = anima.layers[0].stateMachine;
        //for (int i = 0; i < stateMachine.states.Length; i++)
        //{
        //    var item = stateMachine.states[i].state;
        //    m_StateToAssetName.Add(item.name, item.motion.name);
        //}

    }

    public async UniTaskVoid OverrideAnima(ESkillIndex f_SkillIndex, EPersonSkillType f_PersonSkill)
    {
        var stateName = f_SkillIndex.ToString();
        if (!m_StateToAssetName.TryGetValue(stateName, out var targetName))
        {
            LogError($"原始动画不存在 1, name = {stateName}");
            return;
        }
        var originalAnim = m_AnimOverCon[targetName];
        List<KeyValuePair<AnimationClip, AnimationClip>> listClip = new();
        m_AnimOverCon.GetOverrides(listClip);
        if (originalAnim == null)
        {
            LogError($"原始动画不存在 2, name = {targetName}");
            return;
        }
        var skillClip = await DataEntity.LoadSkillAnimtionClip(f_PersonSkill);
        if (skillClip != null)
        {
            m_AnimOverCon[targetName] = skillClip;
            Log($"覆盖新技能， 就技能：{originalAnim.name}, 新技能：{skillClip.name}");
        }
    }
}
