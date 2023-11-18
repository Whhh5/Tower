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

    public class RoationData
    {
        public Vector3 Angle;
    }
    public Dictionary<int, RoationData> WeaponChildRotation = new();

    public override bool IsUpdateEnable => true;

    public override void InitData(int f_ChunkIndex = -1)
    {
        base.InitData(f_ChunkIndex);
        WeaponChildRotation.Clear();
        for (int i = 0; i < WeaponChildCount; i++)
        {
            WeaponChildRotation.Add(i, new());
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (m_CurEffectStopTime > Time.time)
        {
            if (Effect == null)
            {
                Effect = new Entity_Monster_Boss1EffectData();
                Effect.InitData((1 + m_EffectRadius) * (GameDataMgr.MapChunkLength + GameDataMgr.MapChunkInterval.x) * 1.8f);
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

        SetWeaponRootPosition(WeaponStartPos);
        UpdateWeaponChildRotation();
        SetMainWeaponAlpha(MainWeaponAlpha);
        SetWeaponChildAlpha(0.0f);
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

        var curMainAlpha = MainWeaponAlpha;
        var curChildAlpha = WeaponChildAlpha;
        var unitChildAngle = 360.0f / WeaponChildCount;
        await DOTween.To(() => 0.0f, slider =>
        {
            var value = Mathf.Lerp(curMainAlpha, 0.0f, slider);
            SetMainWeaponAlpha(value);
            SetWeaponChildAlpha(slider);
            var childValue = Mathf.Lerp(curChildAlpha, 1.0f, slider * 2);

            foreach (var item in WeaponChildRotation)
            {
                var angle = Mathf.Lerp(0, unitChildAngle * (item.Key + WeaponChildRotation.Count / 2), Mathf.Sin(slider * Mathf.PI * 0.5f));
                SetWeaponChildRotation(item.Key, Vector3.forward * angle);
            }
            UpdateWeaponChildRotation();

        }, 1.0f, atkTime1);

        var curPos = WeaponStartPos;
        var maxDis = Vector2.SqrMagnitude(curPos - endPos);
        var unitChunkSlider = maxDis / aktPath.Count / maxDis;
        var curIndex = -1;
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
        curPos = WeaponRootPos;
        endPos = WeaponStartPos;
        maxDis = Vector2.SqrMagnitude(curPos - endPos);
        await DOTween.To(() => 0.0f, slider =>
        {
            LerpMove(slider);
            LoopRotation();
            SetWeaponChildAlpha(1 - slider);

        }, 1.0f, atkTime4);

        var curAlpha = MainWeaponAlpha;
        await DOTween.To(() => 0.0f, slider =>
        {
            var value = Mathf.Lerp(curAlpha, 1.0f, slider);
            SetMainWeaponAlpha(value);
            LoopRotation();

        }, 1.0f, atkTime5);

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
            SetWeaponRootPosition(pos);
        }
        void LoopRotation()
        {
            foreach (var item in WeaponChildRotation)
            {
                var angle = item.Value.Angle.z + Time.deltaTime * 360 * 3;
                SetWeaponChildRotation(item.Key, Vector3.forward * angle);
            }
            UpdateWeaponChildRotation();
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

    public Vector3 WeaponRootPos;
    public void SetWeaponRootPosition(Vector3 f_Pos)
    {
        WeaponRootPos = f_Pos;
        if (EntityTarget != null)
        {
            EntityTarget.SetWeaponRootPosition();
        }
    }
    private void SetWeaponChildRotation(int f_Index, Vector3 f_Angle)
    {
        if (!WeaponChildRotation.TryGetValue(f_Index, out var data))
        {
            return;
        }
        data.Angle = f_Angle;
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
    public float WeaponChildAlpha = 0.0f;
    private void SetWeaponChildAlpha(float f_Alpha)
    {
        WeaponChildAlpha = f_Alpha;
        if (EntityTarget != null)
        {
            EntityTarget.SetWeaponChildAlpha();
        }
    }
    private void UpdateWeaponChildRotation()
    {
        if (EntityTarget != null)
        {
            EntityTarget.SetWeaponChildRotation();
        }
    }

}
public class Entity_Monster_Boss1 : Entity_Monster_MarriorBase
{
    public new Entity_Monster_Boss1Data EntityData => GetData<Entity_Monster_Boss1Data>();
    [SerializeField]
    private List<SpriteRenderer> m_MainWeaponList = new();
    [SerializeField]
    private Transform m_WeaponsRoot = null;
    [SerializeField]
    private Transform m_WeaponChildItem = null;
    private List<Transform> m_WeaponChilds = new();

    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();


    }
    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        await base.OnStartAsync(f_Params);
        m_WeaponChildItem.gameObject.SetActive(false);
        for (int i = 0; i < EntityData.WeaponChildCount; i++)
        {
            var item = GameObject.Instantiate(m_WeaponChildItem, m_WeaponChildItem.parent);
            item.gameObject.SetActive(true);
            m_WeaponChilds.Add(item);
        }
        SetWeaponRootPosition();
        SetWeaponChildRotation();
        SetMainWeaponAlpha();
        SetWeaponChildAlpha();
    }
    public override async UniTask OnUnLoadAsync()
    {
        foreach (var item in m_WeaponChilds)
        {
            GameObject.Destroy(item.gameObject);
        }
        m_WeaponChilds.Clear();
        await base.OnUnLoadAsync();
    }
    public void SetWeaponChildRotation()
    {
        foreach (var item in EntityData.WeaponChildRotation)
        {
            m_WeaponChilds[item.Key].eulerAngles = item.Value.Angle;
        }
    }
    public void SetWeaponRootPosition()
    {
        m_WeaponsRoot.position = EntityData.WeaponRootPos;
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
    public void SetWeaponChildAlpha()
    {
        foreach (var item in m_WeaponChilds)
        {
            var coms = item.gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (var com in coms)
            {
                var color = com.color;
                color.a = EntityData.WeaponChildAlpha;
                com.color = color;
            }
        }
    }
}

