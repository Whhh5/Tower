using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity_Monster_Default2Data : Person_EnemyData
{
    public Entity_Monster_Default2Data(int f_Index, int f_TargetIndex, Entity_SpawnPointMonsterData f_TargetSpawnPoint) : base(f_Index, f_TargetIndex, f_TargetSpawnPoint)
    {
    }
    public override AssetKey AssetPrefabID => AssetKey.Entity_Monster_Default2;

    protected override int AtkRange => base.AtkRange + 1;

    public override ELayer LayerMask => ELayer.Enemy;

    public override ELayer AttackLayerMask => ELayer.Player;

    public override void AfterLoad()
    {
        base.AfterLoad();

    }
}
public class Entity_Monster_Default2 : Person_Enemy
{
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
