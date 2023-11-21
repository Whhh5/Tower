using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity_Hero_EnchanterBaseData : Entity_HeroBaseNewData
{
    public override int AtkRangeBase => 5;
    public new Entity_Hero_EnchanterBase EntityTarget => GetCom<Entity_Hero_EnchanterBase>();



    public override void AttackBehavior()
    {
        base.AttackBehavior();

        var goTime = 0.4f;
        bool isAtked = false;

        var curWeaponPos = GetWeaponPosition();
        var curWeaponUp = GetWeaponUp();

        var goPos = curWeaponPos + Vector3.right * 0.3f;
        var goUp = (WeaponStartUp + new Vector3(1.7f, 1.0f)).normalized;

        Vector3 pos = Vector3.zero;
        Vector3 up = Vector3.zero;
        GTools.AudioMgr.PlayAudio(EAudioType.Hero_Enchanter1_Attack1);
        DOTween.To(() => 0.0f, slider =>
        {
            if (slider < goTime)
            {
                var value = slider / goTime;
                pos = Vector3.Lerp(curWeaponPos, goPos, value);
                up = Vector3.Lerp(curWeaponUp, goUp, value);
            }
            else
            {
                if (!isAtked)
                {
                    isAtked = true;
                    curWeaponPos = GetWeaponPosition();
                    curWeaponUp = GetWeaponUp();
                    CreateEffect();
                }

                var value = (slider - goTime) / (1 - goTime);
                pos = Vector3.Lerp(curWeaponPos, WeaponStartPos, value);
                up = Vector3.Lerp(curWeaponUp, WeaponStartUp, value);
            }
            pos.z = WeaponStartPos.z;
            up.z = WeaponStartUp.z;
            UpdateWeaponPosition(pos);
            UpdateWeaponDirectionUp(up);

        }, 1.0f, AtkBehavior)
            .SetId(DGID_Atk);

    }
    private async void CreateEffect()
    {
        var startPos = GetAtkPoint();
        var targetPos = m_CurAttackTarget.WorldPosition;
        var moveTime = Vector3.Distance(startPos, targetPos) * 0.3f;
        var effect = new Entity_Hero_EnchanterAttackEffectData();
        effect.SetPosition(startPos);

        GTools.AudioMgr.PlayAudio(EAudioType.Hero_Enchanter1_Attack2);
        DOTween.To(() => 0.0f, slider =>
        {
            if (GTools.UnityObjectIsVaild(m_CurAttackTarget))
            {
                targetPos = m_CurAttackTarget.WorldPosition;
            }
            var pos = Vector3.Lerp(startPos, targetPos, slider);
            effect.SetPosition(pos);

        }, 1.0f, moveTime)
            .SetId(DGID_Move)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                effect.DestroyAsync();
                this.EntityDamage(m_CurAttackTarget, null, AtkAddMagic);
                GTools.AudioMgr.PlayAudio(EAudioType.Hero_Enchanter1_Attack3);
            });

        await ILoadPrefabAsync.LoadAsync(effect);
    }
    public Vector3 GetAtkPoint()
    {
        if (EntityTarget != null)
        {
            return EntityTarget.GetAtkPoint();
        }
        return WorldPosition;
    }
}
public abstract class Entity_Hero_EnchanterBase : Entity_HeroBaseNew
{
    [SerializeField]
    private Transform m_PointAtk;
    public Vector3 GetAtkPoint()
    {
        return m_PointAtk.position;
    }

}