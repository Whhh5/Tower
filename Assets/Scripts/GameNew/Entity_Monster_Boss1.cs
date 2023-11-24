using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity_Monster_Boss1Data : Entity_Monster_WarriorBaseData
{
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Monster_Boss1;
    public new Entity_Monster_Boss1 EntityTarget => GetCom<Entity_Monster_Boss1>();
    public override int AtkRangeBase => 1;
    public override int AtkAddMagic => 100;
    protected override bool IsSkill => true;
    public int SkillRange => 4;
    public int SkillDamage => CurHarm * 2;
    public float SkillTime = 4.0f;
    protected override float AtkSpeedBase => 1f;
    public override int HarmBase => 100;
    public override int MaxBloodBase => 10000;
    public int WeaponChildCount => 5;
    private Entity_Monster_Boss1EffectData Effect = null;

    private float m_CurEffectStopTime = 0.0f;
    private float m_CurEffectDuration => 5.0f;
    private float m_LastEffectAtkTime = 0.0f;
    private int m_EffectRadius => 2;
    private float m_EffectAtkInterval => 0.2f;
    private int m_EffectAtkHarm => 2;


    public override bool IsUpdateEnable => true;

    public override void InitData(int f_ChunkIndex = -1)
    {
        base.InitData(f_ChunkIndex);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (m_CurEffectStopTime > Time.time)
        {
            if (Effect == null)
            {
                Effect = new Entity_Monster_Boss1EffectData();
                Effect.InitData((1 + m_EffectRadius) * (GameDataMgr.MapChunkSize.x + GameDataMgr.MapChunkInterval.x) * 1.8f);
                GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(Effect));
            }

            Effect.SetPosition(WorldPosition);

            if (Time.time - m_LastEffectAtkTime < m_EffectAtkInterval)
            {
                return;
            }
            m_LastEffectAtkTime = Time.time;
            if (!GTools.CreateMapNew.TryGetRangeChunkByIndex(CurrentIndex, out var list, chunkIndex=>
            {
                if (!GTools.CreateMapNew.TryGetChunkData(chunkIndex, out var chunkData))
                {
                    return false;
                }
                if (chunkData.IsExistObj(AttackLayerMask, out var list))
                {
                    return true;
                }
                return false;
            }, true, m_EffectRadius))
            {
                return;
            }
            foreach (var item in list)
            {
                if (!GTools.CreateMapNew.TryGetChunkData(item, out var chunkData))
                {
                    continue;
                }
                if (!chunkData.IsExistObj(AttackLayerMask, out var targets))
                {
                    continue;
                }
                foreach (var target in targets)
                {
                    if (target is not WorldObjectBaseData objData)
                    {
                        continue;
                    }
                    this.EntityDamage(objData, -m_EffectAtkHarm);
                }
            }

        }
        else if (Effect != null)
        {
            Effect.Destroy();
            Effect = null;
        }

    }
    public override void OnLoad()
    {
        base.OnLoad();

    }
    public override void AfterLoad()
    {
        base.AfterLoad();

        SetMainWeaponAlpha(MainWeaponAlpha);
    }
    public override void UnLoad()
    {
        base.UnLoad();
        if (Effect != null)
        {
            Effect.Destroy();
            Effect = null;
        }
    }
    public override void AttackBehavior()
    {
        base.AttackBehavior();
        m_CurEffectStopTime = Time.time + m_CurEffectDuration;
    }

    public override async void SkillBehavior()
    {
        var atkTime1 = SkillTime * 0.8f * 0.1f;
        var atkTime2 = SkillTime * 0.8f * 0.4f;
        var atkTime3 = SkillTime * 0.8f * 0.1f;
        var atkTime4 = SkillTime * 0.8f * 0.3f;
        var atkTime5 = SkillTime * 0.8f * 0.1f;

        var enemyIndex = m_CurAttackTarget.CurrentIndex;
        if (!GTools.CreateMapNew.GetDirectionByIndex(CurrentIndex, enemyIndex, out var dir))
        {
            return;
        }
        var tempIndex = CurrentIndex;
        List<int> aktPath = new();
        aktPath.Add(CurrentIndex);
        for (int i = 0; i < SkillRange; i++)
        {
            if (!GTools.CreateMapNew.GetDirectionChunk(tempIndex, dir, out var index))
            {
                continue;
            }
            tempIndex = index;
            aktPath.Add(index);
        }
        if (aktPath.Count == 1)
        {
            return;
        }
        if (!GTools.CreateMapNew.TryGetChunkData(tempIndex, out var chunkData))
        {
            return;
        }
        var endPos = chunkData.WorldPosition;

        var effectItem = new Entity_Monster_Boss1SkillItemData();
        effectItem.SetPosition(WeaponStartPos);
        effectItem.SetRootPosition(WeaponStartPos);
        effectItem.InitItemList(WeaponChildCount);
        GTools.RunUniTask(ILoadPrefabAsync.LoadAsync(effectItem));


        var curMainAlpha = MainWeaponAlpha;
        var curChildAlpha = effectItem.GetCurItemAlpha();
        var unitChildAngle = 360.0f / WeaponChildCount;
        GTools.AudioMgr.PlayAudio(EAudioType.Monster_Boss1_Skill4);
        await DOTween.To(() => 0.0f, slider =>
        {
            var value = Mathf.Lerp(curMainAlpha, 0.0f, slider);
            SetMainWeaponAlpha(value);

            effectItem.SetItemsAlpha(slider);
            for (int i = 0; i < WeaponChildCount; i++)
            {
                var angle = Mathf.Lerp(0, unitChildAngle * (i + WeaponChildCount / 2), Mathf.Sin(slider * Mathf.PI * 0.5f));
                effectItem.SetItemRotation(i, angle);
            }

        }, 1.0f, atkTime1);

        var curPos = WeaponStartPos;
        var maxDis = Vector2.SqrMagnitude(curPos - endPos);
        var unitChunkSlider = maxDis / aktPath.Count / maxDis;
        var curIndex = -1;
        GTools.AudioMgr.PlayAudio(EAudioType.Monster_Boss1_Skill5);
        await DOTween.To(() => 0.0f, slider =>
        {
            LerpMove(slider);
            LoopRotation();
        }, 1.0f, atkTime2);


        await DOTween.To(() => 0.0f, slider =>
        {
            LoopRotation();

        }, 1.0f, atkTime3);
        var maxCount = aktPath.Count;
        for (int i = 0; i < aktPath.Count / 2; i++)
        {
            var item = aktPath[i];
            aktPath[i] = aktPath[aktPath.Count - 1 - i];
            aktPath[aktPath.Count - 1 - i] = item;
        }
        curIndex = aktPath.Count;
        curPos = effectItem.GetWeaponRootPosition();
        endPos = WeaponStartPos;
        maxDis = Vector2.SqrMagnitude(curPos - endPos);
        GTools.AudioMgr.PlayAudio(EAudioType.Monster_Boss1_Skill6);
        await DOTween.To(() => 0.0f, slider =>
        {
            LerpMove(slider);
            LoopRotation();
            effectItem.SetItemsAlpha(1 - slider);

        }, 1.0f, atkTime4);

        var curAlpha = MainWeaponAlpha;
        await DOTween.To(() => 0.0f, slider =>
        {
            var value = Mathf.Lerp(curAlpha, 1.0f, slider);
            SetMainWeaponAlpha(value);
            LoopRotation();

        }, 1.0f, atkTime5);
        ILoadPrefabAsync.UnLoad(effectItem);

        void LerpMove(float slider)
        {
            Vector3 pos = Vector2.Lerp(curPos, endPos, slider);
            pos.z = WeaponStartPos.z;
            var moveIndex = Mathf.FloorToInt(slider / unitChunkSlider);
            if (curIndex != moveIndex)
            {
                var index = aktPath[moveIndex];
                AtkRange(index);
                curIndex = moveIndex;
            }
            effectItem.SetRootPosition(pos);
        }
        void LoopRotation()
        {
            for (int i = 0; i < WeaponChildCount; i++)
            {
                if (!effectItem.TryGetItemData(i, out var data))
                {
                    continue;
                }
                var angle = data.Angle + Time.deltaTime * 360 * 3;
                effectItem.SetItemRotation(i, angle);
            }
        }


        base.SkillBehavior();
    }
    private void AtkRange(int f_Index)
    {
        if (!GTools.CreateMapNew.TryGetRangeChunkByIndex(f_Index, out var list, index =>
        {
            if (!GTools.CreateMapNew.TryGetChunkData(index, out var chunkData))
            {
                return false;
            }
            return chunkData.IsExistObj(AttackLayerMask, out var _);
        }, true, 1))
        {
            return;
        }
        foreach (var item in list)
        {
            if (!GTools.CreateMapNew.TryGetChunkData(item, out var chunkData))
            {
                continue;
            }
            if (!chunkData.IsExistObj(AttackLayerMask, out var targetList))
            {
                continue;
            }
            foreach (var target in targetList)
            {
                if (target is not WorldObjectBaseData objData)
                {
                    continue;
                }
                this.EntityDamage(objData, -SkillDamage);
            }
        }
    }

    
    public float MainWeaponAlpha = 1.0f;
    private void SetMainWeaponAlpha(float f_Alpha)
    {
        MainWeaponAlpha = f_Alpha;
        if (EntityTarget != null)
        {
            EntityTarget.SetMainWeaponAlpha();
        }
    }
    

}
public class Entity_Monster_Boss1 : Entity_Monster_MarriorBase
{
    public new Entity_Monster_Boss1Data EntityData => GetData<Entity_Monster_Boss1Data>();
    [SerializeField]
    private List<SpriteRenderer> m_MainWeaponList = new();

    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();


    }
    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        await base.OnStartAsync(f_Params);
        SetMainWeaponAlpha();
    }
    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
    }

    public void SetMainWeaponAlpha()
    {
        var color = m_MainWeaponList[0].color;
        color.a = EntityData.MainWeaponAlpha;
        foreach (var item in m_MainWeaponList)
        {
            item.color = color;
        }
    }
}

