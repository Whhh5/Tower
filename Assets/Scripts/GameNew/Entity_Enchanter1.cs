using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Enchanter1Data : Entity_Hero_EnchanterBaseData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Enchanter1;

    public int SkillRange => CurAtkRange * 2;
    public Vector2Int SkillAtkCountRange => new Vector2Int(10, 20);
    public float SkillTime => 2.0f;
    public int SkillDamage => Mathf.CeilToInt(CurHarm * 0.3f);

    public override EHeroCardType HeroType => EHeroCardType.Hero2;

    public override async void SkillBehavior()
    {
        // 
        if (this.TryGetRandomEnemy(SkillRange, out var targets))
        {
            foreach (var item in targets)
            {
                AttackTargetOne(item);
            }
        }
        // 施法动作
        var time1 = 1.0f;
        var time2 = 0.7f;
        var time3 = 0.4f;
        var endPos1 = WeaponStartPos + Vector3.up;
        var endPos2 = WeaponStartPos + Vector3.right * 0.5f;
        var startPos = GetWeaponPosition();
        var startRot = Quaternion.Euler(LocalRotation).z;
        var rotAngle = startRot + 360 * 5;

        GTools.AudioMgr.PlayAudio(EAudioType.Hero_Enchanter1_Skill1);
        await DOTween.To(() => 0.0f, slider =>
          {
              var pos = Vector3.Lerp(startPos, endPos1, slider);
              pos.z = WeaponStartPos.z;
              var rot = Mathf.Lerp(startRot, rotAngle, slider);
              rot = Mathf.Deg2Rad * rot;

              var dirY = Mathf.Sin(rot);
              var dirX = Mathf.Cos(rot);

              UpdateWeaponDirectionUp(new Vector3(dirX, dirY, 0));
              UpdateWeaponPosition(pos);

          }, 1.0f, time1)
            .SetId(DGID_Skill);

        var curPos = GetWeaponPosition();
        var curUp = GetWeaponUp();
        await DOTween.To(() => 0.0f, slider =>
          {
              var pos = Vector3.Lerp(curPos, endPos2, slider);
              var up = Vector3.Lerp(curPos, Vector3.right, slider);

              UpdateWeaponDirectionUp(up);
              UpdateWeaponPosition(pos);
          }, 1.0f, time2)
            .SetId(DGID_Skill);

        await UniTask.Delay(300);
        curPos = GetWeaponPosition();
        curUp = GetWeaponUp();
        await DOTween.To(() => 0.0f, slider =>
        {
            var pos = Vector3.Lerp(curPos, WeaponStartPos, slider);
            var up = Vector3.Lerp(curPos, WeaponStartUp, slider);

            UpdateWeaponDirectionUp(up);
            UpdateWeaponPosition(pos);

        }, 1.0f, time3)
            .SetId(DGID_Skill);

        base.SkillBehavior();
    }
    public async void AttackTargetOne(WorldObjectBaseData f_TargetData)
    {
        Vector3 targetPos = f_TargetData.WorldPosition;
        Vector3 GetTargetPos()
        {
            if (GTools.UnityObjectIsVaild(f_TargetData))
            {
                targetPos = f_TargetData.WorldPosition;
            }
            return targetPos;
        }
        var curCount = GTools.MathfMgr.GetRandomValue(SkillAtkCountRange.x, SkillAtkCountRange.y);
        UniTask[] tasks = new UniTask[curCount];
        for (int i = 0; i < curCount; i++)
        {
            tasks[i] = UniTask.Create(async () =>
            {
                var delayTime = GTools.MathfMgr.GetRandomValue(200, 3000);
                await UniTask.Delay(delayTime);

                var offsetX = GTools.MathfMgr.GetRandomValue(-0.8f, -1.8f);
                var offsetY = GTools.MathfMgr.GetRandomValue(0.8f, 1.8f);
                var startPos = GetTargetPos() + new Vector3(offsetX, offsetY);

                offsetX = GTools.MathfMgr.GetRandomValue(-0.5f, 0.5f);
                offsetY = GTools.MathfMgr.GetRandomValue(-0.5f, 0.5f);
                var targetPos = GetTargetPos() + new Vector3(offsetX, offsetY);

                var moveTime = GTools.MathfMgr.GetRandomValue(0.7f, 1.0f);

                var colorR = GTools.MathfMgr.GetRandomValue(0, 1.0f);
                var colorG = GTools.MathfMgr.GetRandomValue(0, 1.0f);
                var colorB = GTools.MathfMgr.GetRandomValue(0, 1.0f);
                var effect = new Entity_Hero_EnchanterEffectData();
                effect.SetPosition(startPos);
                effect.SetMainColor(new Color(colorR, colorG, colorB, 1));
                await ILoadPrefabAsync.LoadAsync(effect);

                GTools.AudioMgr.PlayAudio(EAudioType.Hero_Enchanter1_Skill2);
                await DOTween.To(() => 0.0f, slider =>
                  {
                      var pos = Vector3.Lerp(startPos, targetPos, slider);
                      pos.z = WeaponStartPos.z;
                      effect.SetPosition(pos);


                  }, 1.0f, moveTime)
                    .SetId(DGID_Skill);
                GTools.AudioMgr.PlayAudio(EAudioType.Hero_Enchanter1_Skill3);
                this.EntityDamage(f_TargetData, -SkillDamage);
                effect.DestroyAsync();
            });
        }
        await UniTask.WhenAll(tasks);
    }
}
public class Entity_Enchanter1 : Entity_Hero_EnchanterBase
{

}
