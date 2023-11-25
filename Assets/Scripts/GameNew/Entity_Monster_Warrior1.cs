using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity_Monster_Warrior1Data : Entity_Monster_WarriorBaseData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Monster_Warrior1;
    public override int HarmBase => TableMgr.monsterBaseInfo[0,0];
    public override int MaxBloodBase => TableMgr.monsterBaseInfo[0,1];
    public override int DefenceBase => TableMgr.monsterBaseInfo[0,2];
}
public class Entity_Monster_Warrior1 : Entity_Monster_MarriorBase
{
    
}

