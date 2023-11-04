using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_SpawnPointPlayerData : Entity_SpawnPointData
{
    public Entity_SpawnPointPlayerData(EDirection f_Direction) : base(f_Direction)
    {

    }

    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;

    public override EAssetKey AssetPrefabID => EAssetKey.SpawnPointPlayer1;
}
public class Entity_SpawnPointPlayer1 : Entity_SpawnPoint
{

}
