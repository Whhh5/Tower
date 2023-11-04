using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class Entity_Monster_WarriorBaseData : Entity_MonsterBaseNewData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Monster_Warrior1;
    public override int AtkRangeBase => 1;
    public new Entity_Monster_Warrior1 EntityTarget => GetCom<Entity_Monster_Warrior1>();
    public override async void AttackBehavior()
    {
        base.AttackBehavior();
        var curPos = EntityTarget != null ? EntityTarget.AtkWeaponPos : WeaponStartPos;
        var startPos = curPos;
        var curUp = EntityTarget != null ? EntityTarget.DirectionUp : WeaponStartUp;
        var startUp = curUp;
        bool isArrive = false;
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
                    curPos = EntityTarget != null ? EntityTarget.AtkWeaponPos : WeaponStartPos;
                    curUp = EntityTarget != null ? EntityTarget.DirectionUp : WeaponStartUp;
                    isArrive = true;
                    this.EntityDamage(m_CurAttackTarget, null, -1);
                }
                var goSilder = (value - goValue) / (1 - goValue);
                pos = Vector2.Lerp(curPos, startPos, goSilder);
                up = Vector2.Lerp(curUp, startUp, goSilder);
            }
            pos.z = WeaponStartPos.z;
            up.z = WeaponStartUp.z;
            UpdateWorldPosition(pos);
            UpdateDirectionUp(up);
        }, 1.0f, AtkInterval * 0.8f)
            .SetId(DGID_Atk);
    }
}
public abstract class Entity_Monster_MarriorBase : Entity_MonsterBaseNew
{
    
}
