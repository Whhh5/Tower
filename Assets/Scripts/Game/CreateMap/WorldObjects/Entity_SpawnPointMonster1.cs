using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 怪物洞穴数据
public class Entity_SpawnPointMonsterData : Entity_SpawnPointData
{

    public Entity_SpawnPointMonsterData(EDirection f_Direction) : base(f_Direction)
    {

    }

    public override ELayer LayerMask => ELayer.Enemy;

    public override ELayer AttackLayerMask => ELayer.Player;

    public override EAssetKey AssetPrefabID => EAssetKey.SpawnPointMonster1;
}
public class Entity_SpawnPointMonster1 : Entity_SpawnPoint
{

}
