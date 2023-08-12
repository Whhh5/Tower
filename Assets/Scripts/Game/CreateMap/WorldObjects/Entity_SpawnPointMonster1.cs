using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 怪物洞穴数据
public class Entity_SpawnPointMonsterData : Entity_SpawnPointData
{

    public Entity_SpawnPointMonsterData(int f_Index, int f_ChunkIndex, EDirection f_Direction) : base(f_Index, f_ChunkIndex, f_Direction)
    {

    }

    public override ELayer LayerMask => ELayer.Enemy;

    public override ELayer AttackLayerMask => ELayer.Player;

    public override AssetKey AssetPrefabID => AssetKey.SpawnPointMonster1;
}
public class Entity_SpawnPointMonster1 : Entity_SpawnPoint
{

}
