using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Player_Default1Data : Person_EnemyData
{
    public Entity_Player_Default1Data(int f_Index, int f_TargetIndex, Entity_SpawnPointPlayerData f_TargetSpawnPoint) : base(f_Index, f_TargetIndex, f_TargetSpawnPoint)
    {
    }

    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;

    public override EHeroCardType CardType => EHeroCardType.Monster_Default1;
    public override AssetKey AssetPrefabID => AssetKey.Entity_Player_Default1;
    public override int HarmBase => 12;

    public override void AnimatorCallback050()
    {
        base.AnimatorCallback050();

        switch (CurStatus)
        {
            case EPersonStatusType.None:
                break;
            case EPersonStatusType.Idle:
                break;
            case EPersonStatusType.Walk:
                break;
            case EPersonStatusType.Attack:
                AttackTarget();
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
}
public class Entity_Player_Default1 : Person_Enemy
{

}
