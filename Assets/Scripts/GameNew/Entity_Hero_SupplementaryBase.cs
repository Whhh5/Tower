using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity_Hero_SupplementaryBaseData : Entity_HeroBaseNewData
{
    public override int AtkRangeBase => 3;
    public override EHeroVocationalType EntityVocationalType => EHeroVocationalType.Supplementary;

    public float ToTargetTime => AtkBehavior * 0.5f;
    public override async void AttackBehavior()
    {
        base.AttackBehavior();


        var targetPos = m_CurAttackTarget.WorldPosition;
        var curPos = GetWeaponPosition();
        var curRotation = GetWeaponRotation();
        var toRotation = curRotation + 720f;
        var dir = (targetPos - curPos).normalized;
        var toDir = new Vector2(dir.y, -dir.x);

        await MainLoop();
        this.EntityDamage(m_CurAttackTarget, null, AtkAddMagic);

        targetPos = WeaponStartPos;
        curPos = GetWeaponPosition();
        curRotation = GetWeaponRotation();
        toRotation = Mathf.Acos(WeaponStartUp.x / Mathf.Pow( Mathf.Pow(WeaponStartUp.x, 2) + Mathf.Pow(WeaponStartUp.y, 2), 0.5f)) * Mathf.Rad2Deg;
        dir = (targetPos - curPos).normalized;
        toDir = new Vector2(dir.y, -dir.x);
        await MainLoop();

        async UniTask MainLoop()
        {
            await DOTween.To(() => 0.0f, slider =>
            {
                var lerpPos = Vector2.Lerp(curPos, targetPos, slider);
                var sinValue = Mathf.Sin(slider * Mathf.PI);
                var addValue = sinValue * toDir;
                Vector3 pos = lerpPos + addValue;
                pos.z = WeaponStartPos.z;
                UpdateWeaponPosition(pos);

                var rot = Mathf.Lerp(curRotation, toRotation, slider);
                var dirX = Mathf.Cos(rot);
                var dirY = Mathf.Sin(rot);
                UpdateWeaponDirectionUp(new Vector3(dirX, dirY, 0));

            }, 1.0f, ToTargetTime)
                .SetId(DGID_Atk);
        }

    }
}
public abstract class Entity_Hero_SupplementaryBase : Entity_HeroBaseNew
{


}