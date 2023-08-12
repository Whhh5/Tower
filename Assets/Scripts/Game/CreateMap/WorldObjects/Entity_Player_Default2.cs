using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Entity_Player_Default2_AttackEffect<T> : EffectMoveBaseData
    where T : TestTimeLineData
{
    public Entity_Player_Default2_AttackEffect(T f_EffectData, float f_UnitSpeed = 1) : base(f_EffectData, f_EffectData.WorldPosition, f_EffectData.TargetEnemy.CentralPoint, f_UnitSpeed)
    {
        m_Initiator = f_EffectData.Initiator;
        m_TargetEnemy = f_EffectData.TargetEnemy;
    }
    public T Effect = null;
    private WorldObjectBaseData m_Initiator = null;
    private WorldObjectBaseData m_TargetEnemy = null;
    public override bool GetExecuteCondition()
    {
        var result = Vector3.Magnitude(TargetPosition - CurPosition) > 0.001f;
        return result;
    }

    public override Vector3? GetPosition()
    {
        return m_TargetEnemy != null ? m_TargetEnemy.CentralPoint : TargetPosition;
    }
}
public class Entity_Player_Default2Data : Person_EnemyData
{
    public Entity_Player_Default2Data(int f_Index, int f_TargetIndex, Entity_SpawnPointPlayerData f_TargetSpawnPoint) : base(f_Index, f_TargetIndex, f_TargetSpawnPoint)
    {
    }
    public override AssetKey AssetPrefabID => AssetKey.Entity_Player_Default2;

    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;

    protected override int AtkRange => base.AtkRange + 1;

    private Entity_Player_Default2 Target => GetCom<Entity_Player_Default2>();
    public override int HarmBase => 15;

    protected override float AtkSpeed => 8.0f;

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

                    var eff = new TestTimeLineData(0, this, startPos, m_CurTarget, -HarmBase, false, DirectorWrapMode.Loop);
                    var data = new Entity_Player_Default2_AttackEffect<TestTimeLineData>(eff, AtkSpeed);
                    GTools.RunUniTask(data.StartExecute());
                }
                break;
            default:
                break;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
public class Entity_Player_Default2 : Person_Enemy
{
    public Entity_Monster_Default1Data ObjectData => TryGetData<Entity_Monster_Default1Data>(out var data) ? data : null;
    public Transform AttackPoint = null;
}
