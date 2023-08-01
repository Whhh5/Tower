using System;
using System.Collections;
using System.Collections.Generic;
using B1;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;

/// <summary>
/// 目录
/// <para />    
/// <para />    <exception cref="Initialization"></exception>
/// <para />    <paramref name="name"/>
/// 
/// <para />    <see cref="Initialization"/>
/// <para />    <see cref="GetSpherePoint"/>
/// <para />    <see cref="NearerTarget"/>
/// <para />    <see cref="GetTargets_Sphere"/>
/// <para />    <see cref="GetBezierValue"/>
/// <para />    <see cref="GetRandomValue"/>
/// <para /><returns></returns>
/// </summary>

public class MathfMgr : Singleton<MathfMgr>
{
    // 随机数索引
    private int m_CurFloatIndex = 0;
    private int m_MaxCount => GTools.RandomInitCount;
    private List<float> m_RandomNumList = null;

    private Collider[] m_ArrCollider = new Collider[GTools.PhysicsOverlapBoxCount];

    /// <summary>
    /// 初始化
    /// </summary>
    public void Initialization()
    {
        m_CurFloatIndex = 0;

        m_RandomNumList = new(new float[m_MaxCount]);
        for (int i = 0; i < m_MaxCount; i++)
        {
            var vlaue = UnityEngine.Random.Range(0.0f, 1.0f);
            m_RandomNumList[i] = vlaue;
        }

    }

    /// <summary>
    /// 获得圆形上的一个点
    /// </summary>
    /// <param name="f_Angle"></param>
    /// <param name="f_Radius"></param>
    /// <returns></returns>
    public Vector2 GetSpherePoint(float f_Angle, float f_Radius = 1)
    {
        var sinValue = Mathf.Sin(f_Angle * Mathf.Deg2Rad);
        var cosValue = Mathf.Cos(f_Angle * Mathf.Deg2Rad);

        return new Vector2(cosValue, sinValue) * f_Radius;
    }
    /// <summary>
    /// 获得圆形范围内最近的目标
    /// </summary>
    /// <param name="f_FromPoint"></param>
    /// <param name="f_Radius"></param>
    /// <param name="f_Layer"></param>
    /// <returns></returns>
    public EntityData NearerTarget(Vector3 f_FromPoint, float f_Radius, ELayer f_Layer)
    {
        var targets = GetTargets_Sphere(f_FromPoint, f_Radius, f_Layer);
        EntityData result = null;
        float curDistance = float.MaxValue;
        float dis;
        foreach (var item in targets)
        {
            dis = Vector3.SqrMagnitude(f_FromPoint - item.CentralPoint);
            if (dis < curDistance)
            {
                curDistance = dis;
                result = item;
            }
        }
        return result;
    }


    public List<EntityData> GetTargets_Sphere(Vector3 f_FromPoint, float f_Radius, ELayer f_Layer)
    {
        Collider[] colliders = Physics.OverlapSphere(f_FromPoint, f_Radius, (int)f_Layer);
        List<EntityData> entitys = new();
        foreach (var item in colliders)
        {
            if (item.TryGetComponent<Entity>(out var icom))
            {
                entitys.Add(icom.EntityData);
            }
        }
        return entitys;
    }
    public List<T> GetTargets_Sphere<T>(Vector3 f_FromPoint, float f_Radius, ELayer f_Layer, Func<T, bool> f_Condition) where T : EntityData
    {
        Collider[] colliders = Physics.OverlapSphere(f_FromPoint, f_Radius, (int)f_Layer);
        List<T> entitys = new();
        foreach (var item in colliders)
        {
            if (item.TryGetComponent<T>(out var icom))
            {
                if (icom.CurStatus != EPersonStatusType.Die)
                {
                    if (f_Condition.Invoke(icom))
                    {
                        entitys.Add(icom);
                    }
                }
            }
        }
        return entitys;
    }
    public List<T> GetOverlapBox<T>(Vector3 f_WorldPos, Vector3 f_HalfExtent, Quaternion f_Rotation, ELayer f_LayerMask, Func<T, bool> f_Condition) where T : Entity
    {
        Collider[] colliders = Physics.OverlapBox(f_WorldPos, f_HalfExtent, f_Rotation, (int)f_LayerMask);
        List<T> data = new();
        for (int i = 0; i < colliders.Length; i++)
        {
            var item = colliders[i];
            if (item.TryGetComponent<T>(out var targetCom) && targetCom.IsActively && f_Condition.Invoke(targetCom))
            {
                data.Add(targetCom);
            }
            m_ArrCollider[i] = null;
        }
        return data;
    }
    /// <summary>
    /// 三次贝塞尔曲线
    /// </summary>
    /// <param name="f_StartPoint"></param>
    /// <param name="f_TargetPoint"></param>
    /// <param name="f_FormLength"></param>
    /// <param name="f_ToLength"></param>
    /// <param name="f_Ratio"></param>
    /// <returns></returns>
    public Vector3 GetBezierValue(Vector3 f_StartPoint, Vector3 f_TargetPoint, Vector3 f_FormLength, Vector3 f_ToLength, float f_Ratio)
    {
        var controlPoint2 = f_StartPoint + f_FormLength;
        var controlPoint3 = f_TargetPoint + f_ToLength;

        var controlPoint1 = f_StartPoint + f_FormLength * f_Ratio;
        var controlPoint4 = f_TargetPoint + f_ToLength - f_ToLength * f_Ratio;





        var point1 = controlPoint1 + (controlPoint2 - controlPoint1) * f_Ratio;

        var point2 = controlPoint2 + (controlPoint3 - controlPoint2) * f_Ratio;

        var point3 = controlPoint3 + (controlPoint4 - controlPoint3) * f_Ratio;

        var endPoint1 = point1 + (point2 - point1) * f_Ratio;

        var endPoint2 = point2 + (point3 - point2) * f_Ratio;

        var targetPoint = endPoint1 + (endPoint2 - endPoint1) * f_Ratio;

        return targetPoint;
    }
    /// <summary>
    /// 获得一个提前预算好的伪随机数
    /// </summary>
    /// <param name="f_Min"></param>
    /// <param name="f_Max"></param>
    /// <returns></returns>
    public float GetRandomValue(float f_Min = 0, float f_Max = 1)
    {
        var curValue = m_RandomNumList[m_CurFloatIndex++];
        m_CurFloatIndex %= m_MaxCount;

        return curValue * (f_Max - f_Min) + f_Min;
    }
    public int GetRandomValue(int f_Min = 0, int f_Max = 100)
    {
        var curValue = m_RandomNumList[m_CurFloatIndex++];
        m_CurFloatIndex %= m_MaxCount;

        return (int)(curValue * (f_Max - f_Min)) + f_Min;
    }




    public bool GetCriticalType(ref EDamageType f_DamageType)
    {
        bool isChange = false;
        switch (f_DamageType)
        {
            case EDamageType.None:
                break;
            case EDamageType.Physical:
                f_DamageType = EDamageType.PhCritical;
                isChange = true;
                break;
            case EDamageType.PhCritical:
                break;
            case EDamageType.Magic:
                f_DamageType = EDamageType.MaCritical;
                isChange = true;
                break;
            case EDamageType.MaCritical:
                break;
            case EDamageType.True:
                break;
            case EDamageType.AddBlood:
                break;
            default:
                break;
        }
        return isChange;
    }
    public bool GetCriticalValue(EntityData f_Initiator, ref EDamageType f_DamageType, ref int f_DamageValue)
    {
        var isCritica = false;
        var rangeNum = GTools.MathfMgr.GetRandomValue(0, 1.0f);
        if (rangeNum < f_Initiator.CriticalChance)
        {
            f_DamageValue = Mathf.CeilToInt(f_DamageValue * f_Initiator.CriticalMultiple);
            GetCriticalType(ref f_DamageType);
            isCritica = true;
        }
        return isCritica;
    }
    public void EntityDamage(EntityData f_Initiator, EntityData f_Target, EDamageType f_DamageType, int f_Value)
    {
        if (f_Target != null && f_Target.CurStatus != EPersonStatusType.Die)
        {

            var hitCondition = f_Target.IsHitConditoin();
            var damageValue = f_Value;
            string hintTex;
            if (hitCondition.Result == EResult.Succeed)
            {
                GetCriticalValue(f_Initiator, ref f_DamageType, ref f_Value);

                var data = new ChangeBloodData()
                {
                    Initiator = f_Initiator,
                    Target = f_Target,
                    ChangeValue = damageValue,
                    EDamageType = f_DamageType,
                };
                f_Target.ChangeBlood(data);
                hintTex = $"{damageValue}";
            }
            else
            {
                hintTex = hitCondition.Value;
            }

            WorldMgr.Ins.DamageText(hintTex, f_DamageType, f_Target.CentralPoint);
        }
    }
}
