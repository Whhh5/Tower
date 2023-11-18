using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Formation_Sphere_EffectData : UnityObjectData
{
    public Entity_Formation_Sphere_EffectData() : base(0)
    {

    }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Formation_Sphere_Effect;
    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;
    private Entity_Formation_Sphere_Effect EntityTarget => GetCom<Entity_Formation_Sphere_Effect>();

    public Vector3 ChildTargetPosition;
    public override void OnLoad()
    {
        base.OnLoad();
    }
    public override void AfterLoad()
    {
        base.AfterLoad();
    }
    public void SetChildTargetPosition(Vector2 f_ToPosition)
    {
        ChildTargetPosition = new Vector3(f_ToPosition.x, f_ToPosition.y, WorldPosition.z);
        if (EntityTarget != null)
        {
            EntityTarget.SetChildTargetPosition();
        }
    }
    public Vector3 GetChildTargetPosition()
    {
        return ChildTargetPosition;
    }
}
public class Entity_Formation_Sphere_Effect : ObjectPoolBase
{
    [SerializeField]
    private Transform m_ChildTarget = null;
    [SerializeField]
    private LineRenderer m_LineRender = null;
    [SerializeField]
    private float m_IntervalPoint = 1;
    [SerializeField]
    private float m_IntervalUpdate = 0.3f;
    private float m_LastTime = 0.0f;

    private Entity_Formation_Sphere_EffectData EntityData => GetData<Entity_Formation_Sphere_EffectData>();

    public void SetChildTargetPosition()
    {
        m_ChildTarget.position = EntityData.ChildTargetPosition;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Time.time - m_LastTime < m_IntervalUpdate)
        {
            return;
        }
        m_LastTime = Time.time;

        var dis = Vector2.Distance(m_ChildTarget.position, transform.position);
        var count = Mathf.FloorToInt(dis / m_IntervalPoint);
        if (count == 0)
        {
            m_LineRender.positionCount = count;
            return;
        }
        var dirFull = m_ChildTarget.position - transform.position;
        var dir = dirFull.normalized;
        var length = 0.0f;
        var minLength = 0.2f;

        var sliderUnit = m_IntervalPoint / dis;
        List<Vector3> positions = new();
        positions.Add(Vector3.zero);
        for (int i = 0; i < count; i++)
        {
            if (1 - length < sliderUnit * 0.8f)
            {
                continue;
            }
            var randomY = GTools.MathfMgr.GetRandomValue(-0.4f, 0.4f);
            var randomX = GTools.MathfMgr.GetRandomValue(sliderUnit * 0.5f, sliderUnit * 0.8f);
            length += randomX;
            Vector2 pos = Vector3.Lerp(transform.position, m_ChildTarget.position, length) - transform.position;
            pos += randomY * new Vector2(dir.y, -dir.x);


            positions.Add(pos);
        }
        positions.Add(m_ChildTarget.position - transform.position);
        m_LineRender.positionCount = positions.Count;
        m_LineRender.SetPositions(positions.ToArray());

    }
}
