using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public class WorldMgr
{
    public static WorldMgr Ins = new();

    public async void DamageText(object f_Danage, EDamageType f_Type, Vector3 f_WorldPos)
    {
        string content = $"{f_Danage}";

        var target = await AssetsMgr.Ins.LoadPrefabPoolAsync<WorldDamageText>(EAssetName.WorldDamageText, null, f_WorldPos, true, null);
        await target.SetParameters(content, f_Type);
    }

    
}
