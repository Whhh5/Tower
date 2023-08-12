using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_SpawnPointPlayerData : Entity_SpawnPointData
{
    public Entity_SpawnPointPlayerData(int f_Index, int f_ChunkIndex, EDirection f_Direction) : base(f_Index, f_ChunkIndex, f_Direction)
    {

    }

    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;

    public override AssetKey AssetPrefabID => AssetKey.SpawnPointPlayer1;
}
public class Entity_SpawnPointPlayer1 : Entity_SpawnPoint
{

}
