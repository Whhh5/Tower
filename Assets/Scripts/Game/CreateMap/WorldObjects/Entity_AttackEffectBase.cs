using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity_AttackEffectBaseData : UnityObjectData, IExecute
{

    public Entity_AttackEffectBaseData() : base(0)
    {
    }

    private float m_TimeCount = 0.2f;
    private Vector3 m_WorldPos = Vector3.zero;
    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    public void Initialization(Vector3 f_WorldPosition)
    {
        m_WorldPos = f_WorldPosition;
    }
    public async UniTask StartExecute()
    {
        await DOTween.To(() => 0, value =>
        {


        }, 1.0f, m_TimeCount);
        await StopExecute();
    }

    public async UniTask StopExecute()
    {

    }
}
public class Entity_AttackEffectBase : ObjectPoolBase
{

}
