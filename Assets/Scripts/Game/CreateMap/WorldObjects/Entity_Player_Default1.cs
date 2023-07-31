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

    public override AssetKey AssetPrefabID => AssetKey.Entity_Player_Default1;
}
public class Entity_Player_Default1 : Person_Enemy
{

}
