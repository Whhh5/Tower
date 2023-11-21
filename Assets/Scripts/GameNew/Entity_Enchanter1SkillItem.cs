using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Enchanter1SkillItemData : UnityObjectData
{
    public Entity_Enchanter1SkillItemData() : base(0)
    {

    }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Enchanter1SkillItem;
    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;
    public Entity_Enchanter1SkillItem EntityTarget => GetCom<Entity_Enchanter1SkillItem>();


}
public class Entity_Enchanter1SkillItem : ObjectPoolBase
{
    public Entity_Enchanter1SkillItemData EntityData => GetData<Entity_Enchanter1SkillItemData>();
}
