using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Hero_Warrior1Data : Entity_Hero_WarriorBaseData
{
    class SkillMoveData
    {
        public Entity_Hero_Warrior1_SkillElementData Element = null;
        public WorldObjectBaseData Target = null;
        public Vector3 Pos1;
        public Vector3 PosOffset;
        public Vector3 TargetOffset;
        public float PosMoveTime;
        public float UpTime;
        public float DelayTime;
        public float PosMoveTime2;
        public float VanishTime;

        private Vector3 m_TempTargetPos = Vector3.zero;
        public Vector3 GetTargetPosition()
        {
            if (GTools.UnityObjectIsVaild(Target))
            {
                m_TempTargetPos = Target.WorldPosition;
            }
            return m_TempTargetPos;
        }
        public async void Load()
        {
            await ILoadPrefabAsync.LoadAsync(Element);
        }
        public void UnLoad()
        {
            ILoadPrefabAsync.UnLoad(Element);
        }
        public void SetPosition(Vector3 f_Position)
        {
            Element.SetPosition(f_Position);
        }
        public void SetUp(Vector3 f_Up)
        {
            Element.SetUp(f_Up);
        }
        public void SetMainColorAlpha(float f_Alpha)
        {
            var color = Element.MainColor;
            color.a = f_Alpha;
            Element.SetMainColor(color);
        }
    }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Hero_Warrior1;

    public int SkillDamage => Mathf.CeilToInt(CurHarm * 0.3f);
    public int SkillCount => 10;
    public int SkillRange => CurAtkRange * 2;
    public float SkillTimeCount => 2.0f / (1 + ((CurAtkSpeed - 1) * 0.5f));

    public override EHeroCardType HeroType => EHeroCardType.Hero1;

    public override async void SkillBehavior()
    {
        List<SkillMoveData> listWeapon = new();
        if (false)
        {
            if (!GTools.CreateMapNew.TryGetRandomTargetByWorldObjectType(CurrentIndex, AttackLayerMask, out var listData, SkillRange))
            {
                return;
            }
            foreach (var item in listData)
            {
                AddAttackEntityOne(item, ref listWeapon);
            }
        }
        else
        {
            AddAttackEntityOne(m_CurAttackTarget, ref listWeapon);
        }


        UniTask[] tasks = new UniTask[listWeapon.Count];
        for (int i = 0; i < listWeapon.Count; i++)
        {
            var item = listWeapon[i];

            tasks[i] = UniTask.Create(async () =>
            {
                item.Load();

                GTools.AudioMgr.PlayAudio(EAudioType.Hero_Warrior1_Skill1);
                var curPosZ = item.Pos1.z;
                var centreOffset = Vector3.up * 0.0f;
                await DOTween.To(() => 0.0f, slider =>
                {
                    var pos = Vector3.Lerp(Vector3.zero, item.PosOffset, slider) + item.Pos1;
                    pos.z = curPosZ;
                    item.SetPosition(pos);
                    item.SetMainColorAlpha(slider);

                }, 1.0f, item.PosMoveTime);

                GTools.AudioMgr.PlayAudio(EAudioType.Hero_Warrior1_Skill2);
                var curPos = item.Pos1 + item.PosOffset;
                await DOTween.To(() => 0.0f, slider =>
                {
                    var pos = Vector3.Lerp(Vector3.zero, Vector3.up * 0.5f, slider) + curPos;
                    pos.z = curPosZ;
                    item.SetPosition(pos);
                    item.SetMainColorAlpha(1 - slider);

                }, 1.0f, item.UpTime)
                    .SetId(DGID_Skill);
                await UniTask.Delay(Mathf.CeilToInt(item.DelayTime * 1000));

                GTools.AudioMgr.PlayAudio(EAudioType.Hero_Warrior1_Skill3);
                item.SetUp(-item.TargetOffset);
                var targetPos = item.GetTargetPosition() + centreOffset;
                await DOTween.To(() => 0.0f, slider =>
                {
                    var pos = item.TargetOffset - Vector3.Lerp(Vector3.zero, item.TargetOffset, slider) + targetPos;
                    pos.z = curPosZ;
                    item.SetPosition(pos);
                    item.SetMainColorAlpha(Mathf.Min(slider / 0.5f, 1.0f));

                }, 1.0f, item.PosMoveTime2)
                    .SetId(DGID_Skill);
                this.EntityDamage(item.Target, -SkillDamage, -1);
                await DOTween.To(() => 0.0f, slider =>
                {
                    item.SetMainColorAlpha(1 - slider);

                }, 1.0f, item.VanishTime)
                    .SetId(DGID_Skill);


                item.UnLoad();
            });

        }
        var curWeaponColor = WeaponColor;
        curWeaponColor.a = 0;
        SetWeaponColor(curWeaponColor);
        await UniTask.WhenAll(tasks);
        await DOTween.To(() => 0.0f, slider =>
        {
            curWeaponColor.a = slider;
            SetWeaponColor(curWeaponColor);
        }, 1.0f, 0.5f);

        base.SkillBehavior();
    }
    private void AddAttackEntityOne(WorldObjectBaseData f_Target, ref List<SkillMoveData> f_TargetData)
    {
        var item = f_Target;
        for (int j = 0; j < SkillCount; j++)
        {
            var element = GetWeaponElement<Entity_Hero_Warrior1_SkillElementData>();
            element.SetUp(WeaponStartUp);
            var xOffset = GTools.MathfMgr.GetRandomValue(-0.2f, 0.5f);
            var yOffset = GTools.MathfMgr.GetRandomValue(-0.2f, 0.5f);
            var moveTime = GTools.MathfMgr.GetRandomValue(0.1f, 0.2f);
            var upTime = GTools.MathfMgr.GetRandomValue(0.1f, 0.3f);
            var delayTime = GTools.MathfMgr.GetRandomValue(0.1f, 1.0f);
            var moveTime2 = GTools.MathfMgr.GetRandomValue(0.2f, 1.5f);
            var vanishTime = GTools.MathfMgr.GetRandomValue(0.1f, 0.3f);
            var xOffset2 = GTools.MathfMgr.GetRandomValue(-0.5f, 0.5f);
            var yOffset2 = GTools.MathfMgr.GetRandomValue(-0.3f, 2.0f);
            f_TargetData.Add(new()
            {
                Element = element,
                Target = item,
                Pos1 = WeaponStartPos,
                PosOffset = new Vector3(xOffset, yOffset, 0),
                PosMoveTime = moveTime / CurAtkSpeed,
                UpTime = upTime,// / CurAtkSpeed,
                DelayTime = delayTime,// / CurAtkSpeed,
                PosMoveTime2 = moveTime2,// / CurAtkSpeed,
                VanishTime = vanishTime,// / CurAtkSpeed,
                TargetOffset = new Vector3(xOffset2, yOffset2, 0),
            });
        }

    }
}
public class Entity_Hero_Warrior1 : Entity_Hero_WarriorBase
{

}
