using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Supplementary1Data : Entity_Hero_SupplementaryBaseData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Supplementary1;

    public override EHeroCardType HeroType => EHeroCardType.Hero3;

    public float SkillTimeCount => 2.0f;
    public float SkillToTime => SkillTimeCount * 0.5f;
    public float SkillFromTime1 => SkillTimeCount * 0.2f;
    public float SkillFromTime2 => SkillTimeCount * 0.2f;
    public float SkillFromTime3 => SkillTimeCount * 0.1f;
    public int SkillRange => CurAtkRange * 1;
    public float UtilWidth => 6f;
    public override int AtkAddMagic => 50;
    public int SkillDamage => Mathf.CeilToInt(CurHarm * 2);


    public override async void SkillBehavior()
    {
        var effect = new Entity_Supplementary1SkillEffectData();
        effect.SetPosition(WorldPosition);
        effect.Play(this);
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(effect));


        var targetPos = WorldPosition;
        var curPos = GetWeaponPosition();
        var curRotation = GetWeaponRotation();
        var toRotation = curRotation + 360 * 5f;
        var curScale = GetWeaponLocalSacle();
        var toScale = UtilWidth * (SkillRange + 1) * WeaponStartScale;

        GTools.AudioMgr.PlayAudio(EAudioType.Hero_Supplementary1_Skill1);
        await MainLoop(SkillToTime);

        curPos = GetWeaponPosition();
        targetPos = curPos;
        curRotation = GetWeaponRotation();
        toRotation = curRotation;
        curScale = GetWeaponLocalSacle();
        toScale = UtilWidth * SkillRange * WeaponStartScale;
        await MainLoop(SkillFromTime1);
        GTools.AudioMgr.PlayAudio(EAudioType.Hero_Supplementary1_Skill2);

        AddRangeEntityBlood();
        await UniTask.Delay(Mathf.CeilToInt(SkillFromTime2 * 1000));

        targetPos = WeaponStartPos;
        curPos = GetWeaponPosition();
        curRotation = GetWeaponRotation();
        toRotation = Mathf.Acos(WeaponStartUp.x / Mathf.Pow(Mathf.Pow(WeaponStartUp.x, 2) + Mathf.Pow(WeaponStartUp.y, 2), 0.5f)) * Mathf.Rad2Deg;
        curScale = GetWeaponLocalSacle();
        toScale = WeaponStartScale;
        await MainLoop(SkillFromTime3);


        async UniTask MainLoop(float time)
        {
            await DOTween.To(() => 0.0f, slider =>
              {
                  Vector3 pos = Vector2.Lerp(curPos, targetPos, slider);
                  pos.z = WeaponStartPos.z;
                  var angle = Mathf.Lerp(curRotation, toRotation, slider);
                  var scale = Vector3.Lerp(curScale, toScale, slider);
                  var rotX = Mathf.Cos(angle);
                  var rotY = Mathf.Sin(angle);

                  UpdateWeaponPosition(pos);
                  UpdateWeaponDirectionUp(new Vector3(rotX, rotY, 0));
                  SetWeaponLocalSacle(scale);

              }, 1.0f, time)
                .SetId(DGID_Skill);
        }






        base.SkillBehavior();
    }

    private async void AddRangeEntityBlood()
    {
        if (this.TryGetRandomTeam(out var list, SkillRange))
        {
            UniTask[] tasks = new UniTask[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                tasks[i] = UniTask.Create(async () =>
                {
                    this.EntityDamage(item, SkillDamage);
                    var effect = new Entity_Effect_AddBloodData();
                    effect.SetPosition(item.WorldPosition);
                    if (item.EntityType == EEntityType.Person)
                    {
                        this.InflictionGain(item, EGainType.AttackSpeed1);
                    }
                    GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(effect));
                    await UniTask.Delay(3000);
                    ILoadPrefabAsync.UnLoad(effect);
                });
            }

            await UniTask.WhenAll(tasks);
        }
    }
}
public class Entity_Supplementary1 : Entity_Hero_SupplementaryBase
{

}
