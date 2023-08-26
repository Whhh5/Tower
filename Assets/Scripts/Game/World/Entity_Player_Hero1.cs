using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity_Player_Hero1Data : Person_EnemyData
{
    public Entity_Player_Hero1Data(int f_Index, int f_TargetIndex, Entity_SpawnPointData f_TargetSpawnPoint) : base(f_Index, f_TargetIndex, f_TargetSpawnPoint)
    {

    }
    public override ELayer LayerMask => ELayer.Player;

    public override ELayer AttackLayerMask => ELayer.Enemy;

    public override AssetKey AssetPrefabID => AssetKey.Entity_Player_Hero1;


    public override void AfterLoad()
    {
        base.AfterLoad();


    }
    public override void OnUnLoad()
    {
        base.OnUnLoad();
    }
    public void Run()
    {

    }
}
public class Entity_Player_Hero1 : Person_Enemy
{
    public Entity_Player_Hero1Data Data => GetData<Entity_Player_Hero1Data>();
    public override async UniTask OnClickAsync()
    {
        await base.OnClickAsync();


    }
    public override async UniTask OnClick2Async()
    {
        await base.OnClick2Async();

    }


    public void Run()
    {

    }
}
