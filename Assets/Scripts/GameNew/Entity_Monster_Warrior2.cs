using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity_Monster_Warrior2Data : Entity_Monster_WarriorBaseData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Monster_Warrior2;
    public override int HarmBase => 50;

    public override bool IsUpdateEnable => true;
    private Entity_Monster_Boss1EffectData Effect = null;

    private float m_CurEffectStopTime = 0.0f;
    private float m_CurEffectDuration => 5.0f;
    private float m_LastEffectAtkTime = 0.0f;
    private int m_EffectRadius => 1;
    private float m_EffectAtkInterval => 0.2f;
    private int m_EffectAtkHarm => 1;
    public override int MaxBloodBase => 2000;
    public override void AttackBehavior()
    {
        base.AttackBehavior();
        m_CurEffectStopTime = Time.time + m_CurEffectDuration;
    }
    public override void UnLoad()
    {
        base.UnLoad();
        if (Effect != null)
        {
            Effect.Destroy();
            Effect = null;
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (m_CurEffectStopTime > Time.time)
        {
            if (Effect == null)
            {
                Effect = new Entity_Monster_Boss1EffectData();
                Effect.InitData((1 + m_EffectRadius) * (GameDataMgr.MapChunkSize.x + GameDataMgr.MapChunkInterval.x) * 1.8f);
                GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(Effect));
            }

            Effect.SetPosition(WorldPosition);

            if (Time.time - m_LastEffectAtkTime < m_EffectAtkInterval)
            {
                return;
            }
            m_LastEffectAtkTime = Time.time;
            if (!GTools.CreateMapNew.TryGetRangeChunkByIndex(CurrentIndex, out var list, chunkIndex =>
            {
                if (!GTools.CreateMapNew.TryGetChunkData(chunkIndex, out var chunkData))
                {
                    return false;
                }
                if (chunkData.IsExistObj(AttackLayerMask, out var list))
                {
                    return true;
                }
                return false;
            }, true, m_EffectRadius))
            {
                return;
            }
            foreach (var item in list)
            {
                if (!GTools.CreateMapNew.TryGetChunkData(item, out var chunkData))
                {
                    continue;
                }
                if (!chunkData.IsExistObj(AttackLayerMask, out var targets))
                {
                    continue;
                }
                foreach (var target in targets)
                {
                    if (target is not WorldObjectBaseData objData)
                    {
                        continue;
                    }
                    this.EntityDamage(objData, -m_EffectAtkHarm);
                }
            }

        }
        else if (Effect != null)
        {
            Effect.Destroy();
            Effect = null;
        }

    }
}
public class Entity_Monster_Warrior2 : Entity_Monster_MarriorBase
{
    
}

