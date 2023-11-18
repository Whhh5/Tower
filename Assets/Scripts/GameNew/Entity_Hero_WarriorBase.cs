using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity_Hero_WarriorBaseData : Entity_HeroBaseNewData
{
    public override int AtkRangeBase => 2;
    public new Entity_Hero_WarriorBase EntityTarget => GetCom<Entity_Hero_WarriorBase>();
    public override async void AttackBehavior()
    {
        base.AttackBehavior();
        var curPos = GetWeaponPosition();
        var curUp = GetWeaponUp();
        bool isArrive = false;
        GTools.AudioMgr.PlayAudio(EAudioType.Hero_Warrior1_Attack1);
        await DOTween.To(() => 0.0f, value =>
        {
            var goValue = 0.7f;
            Vector3 pos;
            Vector3 up;
            if (value < goValue)
            {
                var goSilder = value / goValue;
                pos = Vector2.Lerp(curPos, m_CurAttackTarget.WorldPosition, goSilder);
                up = Vector2.Lerp(curUp, m_CurAttackTarget.WorldPosition - curPos, goSilder);
            }
            else
            {
                if (!isArrive)
                {
                    curPos = GetWeaponPosition();
                    curUp = GetWeaponUp();
                    isArrive = true;
                    this.EntityDamage(m_CurAttackTarget, null, 200);
                    GTools.AudioMgr.PlayAudio(EAudioType.Hero_Warrior1_Attack2);
                }
                var goSilder = (value - goValue) / (1 - goValue);
                pos = Vector2.Lerp(curPos, WeaponStartPos, goSilder);
                up = Vector2.Lerp(curUp, WeaponStartUp, goSilder);
            }
            pos.z = WeaponStartPos.z;
            up.z = WeaponStartUp.z;
            UpdateWeaponPosition(pos);
            UpdateWeaponDirectionUp(up);

        }, 1.0f, AtkBehavior)
            .SetId(DGID_Atk);
    }
}
public abstract class Entity_Hero_WarriorBase : Entity_HeroBaseNew
{
    

}