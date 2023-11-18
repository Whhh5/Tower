using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Formation_NearData : Entity_FormationBaseData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Formation_Near;
    public Entity_Formation_Near EntityTarget => GetCom<Entity_Formation_Near>();


    private Dictionary<int, List<int>> m_CurDetectionIndexList = new();
    private int AttackRange = 6;
    private Vector2 m_EndPos = Vector3.zero;
    private int m_StartIndex = 0;
    private bool m_IsDestroy = false;
    public override void AfterLoad()
    {
        base.AfterLoad();
        m_IsDestroy = false;
        m_StartIndex = FormationData.CentreHero.CurrentIndex;
        var centreIndex = FormationData.CentreHero.CurrentIndex;
        var centreHeroRowCol = GTools.CreateMapNew.IndexToRowCol(centreIndex);

        for (int i = 0; i < AttackRange; i++)
        {
            var row = centreHeroRowCol.x;
            var col = centreHeroRowCol.y + i + 1;

            var index = GTools.CreateMapNew.RowColToIndex(new Vector2Int(row, col));
            if (index < 0)
            {
                continue;
            }
            if (!GTools.CreateMapNew.TryGetChunkData(index, out var chunkData))
            {
                continue;
            }
            m_EndPos = chunkData.WorldPosition;
            List<int> indexList = new();
            m_CurDetectionIndexList.Add(index, indexList);
            if (GTools.CreateMapNew.GetDirectionChunk(index, EDirection.LeftUp, out var leftUpIndex))
            {
                indexList.Add(leftUpIndex);
            }
            if (GTools.CreateMapNew.GetDirectionChunk(index, EDirection.LeftBottom, out var rightIndex))
            {
                indexList.Add(rightIndex);
            }
        }
    }
    public override void UnLoad()
    {
        m_IsDestroy = true;
        base.UnLoad();
    }
    private bool RangeIsEnemy()
    {
        bool one(int index)
        {
            if (!GTools.CreateMapNew.TryGetChunkData(index, out var chunkData))
            {
                return false;
            }
            if (!chunkData.IsExistObj(AttackLayer, out var targetList))
            {
                return false;
            }
            foreach (var item in targetList)
            {
                if (item is WorldObjectBaseData objData && TargetIsVaild(objData))
                {
                    return true;
                }
            }
            return false;
        }
        foreach (var item in m_CurDetectionIndexList)
        {
            if (one(item.Key))
            {
                return true;
            }
            foreach (var index in item.Value)
            {
                if (one(index))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public async void StartExecute()
    {
        var effect = new Entity_Formation_Near_EffectData();
        effect.SetMainColorAlpha(0);
        effect.SetPosition(FormationData.CentreHero.WorldPosition);
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(effect));

        var time = m_IntervalTime * 0.8f;
        Vector2 startPos = effect.WorldPosition;
        await DOTween.To(() => 0.0f, slider =>
            {
                effect.SetMainColorAlpha(slider);

            }, 1.0f, time * 0.5f);
        var curIndex = 0;
        await DOTween.To(() => 0.0f, slider =>
          {
              var pos = Vector3.Lerp(startPos, m_EndPos, slider);
              pos.z = effect.WorldPosition.z;
              effect.SetPosition(pos);

              var alpha = 1 - Mathf.Max((slider - 0.8f) / 0.2f, 0);
              effect.SetMainColorAlpha(alpha);

              //var chunkIndex = (pos.x - startPos.x) / (m_EndPos.x - startPos.x);
              var rangeUtil = 1.0f / AttackRange;
              var index = Mathf.FloorToInt(slider / rangeUtil);
              if (curIndex != index)
              {
                  curIndex = index;
                  var chunkIndex = m_StartIndex + index;
                  AttackChunkIndexEnemy(chunkIndex);
                  if (m_CurDetectionIndexList.TryGetValue(chunkIndex, out var indexList))
                  {
                      foreach (var item in indexList)
                      {
                          AttackChunkIndexEnemy(item);
                      }
                  }
              }

          }, 1.0f, time * 0.5f);
        ILoadPrefabAsync.UnLoad(effect);
    }
    private void AttackChunkIndexEnemy(int f_Index)
    {
        if (GTools.CreateMapNew.TryGetChunkData(f_Index, out var chunkData))
        {
            if (chunkData.IsExistObj(AttackLayer, out var targets))
            {
                foreach (var target in targets)
                {
                    if (target is not WorldObjectBaseData objData || !TargetIsVaild(objData))
                    {
                        continue;
                    }
                    GTools.AttackMgr.EntityDamage(null, objData, -CurHarm);
                }
            }
        }
    }
    private bool TargetIsVaild(WorldObjectBaseData f_ObjData)
    {
        var result = GTools.UnityObjectIsVaild(f_ObjData) && GTools.WorldObjectIsActive(f_ObjData);
        return result;
    }


    private float m_IntervalTime = 1.0f;
    private float m_TempTime = 0.0f;
    public override bool IsUpdateEnable => true;
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Time.time - m_TempTime < m_IntervalTime)
        {
            return;
        }
        m_TempTime = Time.time;
        if (RangeIsEnemy())
        {
            StartExecute();
        }
    }

}
public class Entity_Formation_Near : Entity_FormationBase
{
}
