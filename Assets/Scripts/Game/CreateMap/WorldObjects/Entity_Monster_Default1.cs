using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity_Monster_Default1Data : Person_EnemyData
{
    public Entity_Monster_Default1Data() : base()
    {
    }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Monster_Default1;

    public override ELayer LayerMask => ELayer.Enemy;

    public override ELayer AttackLayerMask => ELayer.Player;

    public override EHeroCardType CardType => EHeroCardType.Monster_Default1;
    public override int HarmBase => 22;

    public override void AfterLoad()
    {
        base.AfterLoad();
    }

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
public class Entity_Monster_Default1 : Person_Enemy
{
    public Entity_Monster_Default1Data ObjectData => TryGetData<Entity_Monster_Default1Data>(out var data) ? data : null;

    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
    }
}
