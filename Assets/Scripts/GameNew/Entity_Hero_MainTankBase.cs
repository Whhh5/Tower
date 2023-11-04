using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity_Hero_MainTankBaseData : Entity_HeroBaseNewData
{
    public override int AtkRangeBase => 1;
    public override EHeroVocationalType EntityVocationalType => EHeroVocationalType.MainTank;
    protected override float AtkSpeedBase => 0;
    public override int MaxBloodBase => base.MaxBloodBase * 5;
    protected override Vector3 WeaponStartPosOffset => new Vector3(0.4f, 0.1f, -2f);
    public override void AttackBehavior()
    {
        base.AttackBehavior();
    }
}
public abstract class Entity_Hero_MainTankBase : Entity_HeroBaseNew
{
    

}