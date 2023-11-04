using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Entity_Player_Hero3Data : Entity_HeroBaseData
{
    public Entity_Player_Hero3Data(EHeroCradStarLevel f_HeroStarLvevl) : base(f_HeroStarLvevl)
    {

    }
    public override EEntityType EntityType => EEntityType.Dragon;
    public Entity_Player_Hero3 EntityMono => GetCom<Entity_Player_Hero3>();

    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Player_Hero3;
    public ESkillStage3 CurrentStage => (ESkillStage3)CurStage;
    public override int SkillStageCount => (int)ESkillStage3.EnumCount;
    public override int AtkRange => 5;
    public override float AtkSpeed => 8;
    public int AddBloodValue = 19;
    public override int HarmBase => 2;
    public override EHeroCardType HeroCradType => EHeroCardType.Hero3;

    public override void AnimatorCallback020()
    {
        base.AnimatorCallback020();

        switch (CurStatus)
        {
            case EPersonStatusType.Skill:
                switch (CurrentStage)
                {
                    case ESkillStage3.Stage3:
                        Skill3();
                        break;
                    default:
                        break;
                }
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
                {
                    var eff = new TestTimeLineData();
                    eff.Initialization(this, AttackPoint, CurAttackTarget, -CurHarm, true, DirectorWrapMode.Loop);
                    var data = new Effect_Track_Line<TestTimeLineData>(eff, AtkSpeed);
                    GTools.RunUniTask(data.StartExecute());
                }
                break;
            case EPersonStatusType.Skill:
                {
                    switch (CurrentStage)
                    {
                        case ESkillStage3.Stage1:
                            Skill1();
                            break;
                        case ESkillStage3.Stage2:
                            Skill2();
                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }
    }
    public override void AnimatorCallback070()
    {
        base.AnimatorCallback070();
        switch (CurStatus)
        {
            case EPersonStatusType.Skill:
                switch (CurrentStage)
                {
                    case ESkillStage3.Stage3:
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    public void Skill1()
    {
        var result = GTools.MathfMgr.GetTargets_Sphere<WorldObjectBaseData>(WorldPosition, AtkRange + 3, LayerMask);
        if (result.Count > 0)
        {
            var tareget = result[0];
            var eff = new Hero3SkillEffectData( );
            eff.Initialization(AttackPoint, AddBloodValue, this);
            var moveData = new Effect_Track_Points<Hero3SkillEffectData>(eff, result, AtkSpeed * 2);
            GTools.RunUniTask(moveData.StartExecute());
        }
    }
    public void Skill2()
    {
        var result = GTools.MathfMgr.GetTargets_Sphere<WorldObjectBaseData>(WorldPosition, AtkRange + 3, LayerMask);
        if (result.Count > 0)
        {
            for (int i = 0; i < 2; i++)
            {
                var speed = AtkSpeed * GTools.MathfMgr.GetRandomValue(2, 3.0f);
                var eff = new Hero3SkillEffectData( );
                eff.Initialization(AttackPoint, AddBloodValue, this);
                var moveData = new Effect_Track_Points<Hero3SkillEffectData>(eff, result, speed);
                GTools.RunUniTask(moveData.StartExecute());
            }
        }
    }
    public async void Skill3()
    {
        var result = GTools.MathfMgr.GetTargets_Sphere<WorldObjectBaseData>(WorldPosition, AtkRange + 3, LayerMask);
        if (result.Count > 0)
        {
            for (int i = 0; i < 5; i++)
            {
                var speed = AtkSpeed * GTools.MathfMgr.GetRandomValue(1.5f, 3.0f);
                var eff = new Hero3SkillEffectData( );
                eff.Initialization(AttackPoint, AddBloodValue, this);
                var moveData = new Effect_Track_Points<Hero3SkillEffectData>(eff, result, speed);
                GTools.RunUniTask(moveData.StartExecute());
                await UniTask.Delay(200);
            }
        }
    }

}
public class Entity_Player_Hero3 : Entity_HeroBase
{
    public Entity_Player_Hero3Data Data => GetData<Entity_Player_Hero3Data>();

}


