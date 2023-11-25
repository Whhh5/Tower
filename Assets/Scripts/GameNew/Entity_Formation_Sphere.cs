using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Entity_Formation_SphereData : Entity_FormationBaseData
{
    private class EffectData
    {
        public WorldObjectBaseData Target;
        public bool IsMoveend = false;
        public float LastAttackTime = 0.0f;
        public int AttackCount = 0;
        public void Reset()
        {
            AttackCount = 0;
            Target = null;
            IsMoveend = false;
        }
    }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Formation_Sphere;
    private Dictionary<int, int> m_CurDetectionIndexList = new();

    Dictionary<Entity_Formation_Sphere_EffectData, EffectData> m_ChildTargets = new();
    private int AttackRange => 10;
    public override void OnLoad()
    {
        base.OnLoad();

        var centreIndex = FormationData.CentreIndex;
        if (GTools.CreateMapNew.TryGetRangeChunkByIndex(centreIndex, out var list, null, false, AttackRange))
        {
            foreach (var item in list)
            {
                m_CurDetectionIndexList.Add(item, item);
            }
        }

        foreach (var item in FormationData.Map)
        {
            var pos = item.Value.GetCurChunkPos();
            var effectData = new Entity_Formation_Sphere_EffectData();
            effectData.SetPosition(pos);
            effectData.SetChildTargetPosition(pos);
            m_ChildTargets.Add(effectData, new());
        }
    }
    public override void AfterLoad()
    {
        base.AfterLoad();
        foreach (var item in m_ChildTargets)
        {
            GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(item.Key));
        }
    }
    public override void UnLoad()
    {
        foreach (var item in m_ChildTargets)
        {
            ILoadPrefabAsync.UnLoad(item.Key);
        }
        m_ChildTargets.Clear();
        m_CurDetectionIndexList.Clear();
        base.UnLoad();
    }
    private float AttackInterval = 0.3f;
    private float AtackHarm = 0.2f;
    public override bool IsUpdateEnable => true;
    public override void OnUpdate()
    {
        base.OnUpdate();


        foreach (var item in m_ChildTargets)
        {
            var effect = item.Key;
            var target = item.Value.Target;
            if (GTools.UnityObjectIsVaild(target) && m_CurDetectionIndexList.ContainsKey(target.CurrentIndex))
            {
                var curPos = effect.GetChildTargetPosition();
                var targetPos = target.WorldPosition;
                if (!item.Value.IsMoveend)
                {
                    var pos = Vector2.MoveTowards(curPos, targetPos, Time.deltaTime * 15);
                    effect.SetChildTargetPosition(pos);
                    if (Vector3.SqrMagnitude(curPos - targetPos) < 0.1f)
                    {
                        item.Value.IsMoveend = true;
                    }
                }
                else
                {
                    effect.SetChildTargetPosition(targetPos);
                    if (Time.time - item.Value.LastAttackTime > AttackInterval)
                    {
                        item.Value.LastAttackTime = Time.time;

                        GTools.AttackMgr.EntityDamage(null, target, -Mathf.CeilToInt(AtackHarm * item.Value.AttackCount++));
                    }
                }
            }
            else
            {
                var curPos = effect.GetChildTargetPosition();
                var pos = Vector2.MoveTowards(curPos, effect.WorldPosition, Time.deltaTime * 15);
                effect.SetChildTargetPosition(pos);
                item.Value.Reset();
                if (TryGetAttackTarget(effect.WorldPosition, out var atkTarget))
                {
                    item.Value.Target = atkTarget;
                }
            }
        }
    }
    public bool TryGetAttackTarget(Vector3 f_Pos, out WorldObjectBaseData f_Target)
    {
        f_Target = null;
        float minDis = float.MaxValue;
        foreach (var item in m_CurDetectionIndexList)
        {
            if (!GTools.CreateMapNew.TryGetChunkData(item.Key, out var chunkData))
            {
                continue;
            }
            if (!chunkData.IsExistObj(ELayer.Enemy, out var list))
            {
                continue;
            }
            foreach (var chunkObjData in list)
            {
                if (chunkObjData is not WorldObjectBaseData objData)
                {
                    continue;
                }
                if (!GTools.UnityObjectIsVaild(objData) || !GTools.WorldObjectIsActive(objData))
                {
                    continue;
                }
                if (f_Target == null)
                {
                    f_Target = objData;
                }
                else
                {
                    var dis = Vector3.SqrMagnitude(f_Pos - f_Target.WorldPosition);
                    if (dis < minDis)
                    {
                        minDis = dis;
                        f_Target = objData;
                    }
                }
            }
        }
        return f_Target != null;
    }
}

public class Entity_Formation_Sphere : Entity_FormationBase
{
    [SerializeField]
    private List<SpriteRenderer> m_MainBodyList = new();


    public override void OnUpdate()
    {
        base.OnUpdate();

        var value = Time.time % 1.0f;
        var alpha = Mathf.Abs(value - 0.5f) * 2 * 0.3f + 0.5f;
        foreach (var item in m_MainBodyList)
        {
            var color = item.color;
            color.a = alpha;
            item.color = color;
        }
    }
}
