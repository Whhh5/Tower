using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Entity_Monster_Default2Data : Person_EnemyData
{
    public Entity_Monster_Default2Data(int f_Index, int f_TargetIndex, Entity_SpawnPointMonsterData f_TargetSpawnPoint) : base(f_Index, f_TargetIndex, f_TargetSpawnPoint)
    {
    }
    public override AssetKey AssetPrefabID => AssetKey.Entity_Monster_Default2;

    protected override int AtkRange => base.AtkRange + 1;

    public override ELayer LayerMask => ELayer.Enemy;

    public override ELayer AttackLayerMask => ELayer.Player;
    private Entity_Monster_Default2 Target => GetCom<Entity_Monster_Default2>();

    protected override float AtkSpeed => 8;
    public override int HarmBase => 33;
    public override void AfterLoad()
    {
        base.AfterLoad();

    }

    public override void AnimatorCallback070()
    {
        base.AnimatorCallback070();

        switch (CurStatus)
        {
            case EPersonStatusType.Attack:
                {
                    var startPos = Target != null && Target.AttackPoint != null ? Target.AttackPoint.position : WorldPosition;

                    var eff = new TestTimeLineData();
                    eff.Initialization(this, startPos, m_CurTarget, -CurHarm, false, DirectorWrapMode.Loop);
                    var data = new Effect_Track_Line<TestTimeLineData>(eff, AtkSpeed);
                    GTools.RunUniTask(data.StartExecute());
                }
                break;
            default:
                break;
        }
    }
}
public class Entity_Monster_Default2 : Person_Enemy
{
    public Transform AttackPoint;

    public Entity_Monster_Default1Data ObjectData => TryGetData<Entity_Monster_Default1Data>(out var data) ? data : null;
    public override void OnUpdate()
    {
        base.OnUpdate();

    }

    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
    }
}
