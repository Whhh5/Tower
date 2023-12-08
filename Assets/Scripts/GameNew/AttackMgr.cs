using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public class AttackMgr : Singleton<AttackMgr>
{
    public void EntityDamage(
        WorldObjectBaseData f_Initiator,
        WorldObjectBaseData f_Target,
        int f_Value,
        int f_AddMagicValue = -1)
    {
        f_Initiator ??= GTools.MonsterMgr.GodEntityData;
        //f_Value = 1;
        if (GTools.UnityObjectIsVaild(f_Target))
        {

            var hitCondition = f_Target.IsHitConditoin();
            var damageValue = f_Value < 0 ? Mathf.Min(-1, f_Value + f_Target.CurDefence) : f_Value;
            string hintTex;
            var value = 0;
            if (hitCondition.Result == EResult.Succeed)
            {
                var data = new ChangeBloodData()
                {
                    Initiator = f_Initiator,
                    Target = f_Target,
                    ChangeValue = damageValue,
                };
                f_Target.ChangeBlood(data);
                WorldWindowMgr.Ins.UpdateBloodHint(f_Target);
                hintTex = $"{damageValue}";
                value = damageValue;

                f_Initiator.ExecuteGainAsync(EBuffView.Collect);
            }
            else
            {
                hintTex = hitCondition.Value;
            }
            if (f_AddMagicValue > 0)
            {
                f_Initiator.ChangeMagic(f_AddMagicValue);
                WorldWindowMgr.Ins.UpdateBloodHint(f_Initiator);
            }

            var xOffset = GTools.MathfMgr.GetRandomValue(-0.3f, 0.3f);
            var yOffset = GTools.MathfMgr.GetRandomValue(-0.3f, 0.3f);
            EDamageType damageType;
            if (damageValue < 0)
            {
                damageType = EDamageType.Physical;
                PlayAttackEffect(f_Target.WorldPosition);
            }
            else
            {
                damageType = EDamageType.AddBlood;
            }
            //WorldMgr.Ins.DamageText(hintTex, damageType, f_Target.CentralPoint + new Vector3(xOffset, yOffset));

            var uguiPos = IUIUtil.GetUGUIPosByWorld(f_Target.WorldPosition, true);
            var worldPos = IUIUtil.GetWorldPosBuyUGUIPos(uguiPos);
            WorldWindowMgr.Ins.CreateDamageHint(damageValue, worldPos);
        }
    }
    public void PlayAttackEffect(Vector3 f_Position)
    {
        var effectData = new Entity_AttackEffect1Data();
        effectData.SetPosition(f_Position);
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(effectData));
    }
}
