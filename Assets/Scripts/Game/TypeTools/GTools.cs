using B1;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using B1.UI;

public static class GTools
{
    #region 游戏设置
    // 当前游戏最大帧数
    public const int MaxFrameRate = 60;
    // 世界空间预制体 update 等级
    public const EUpdateLevel PrefabPoolUpdateLevel = EUpdateLevel.Level2;
    // ui 预制体 update 等级
    public const EUpdateLevel UIWindowUpdateLevel = EUpdateLevel.Level3;
    #endregion


    #region 参考值
    // 池预制体默认初始化数量（场景物体）
    public const uint PrefabPoolDefaultCount = 10;
    // 池预制体默认初始化数量（ui 对象）
    public const uint UIPrefabPoolDefaultCount = 10;
    // 随机数初始化数量
    public const int RandomInitCount = 200;
    // 随机数初始化数量
    public const ushort PhysicsOverlapBoxCount = 10;
    // update 等级参考表
    public static Dictionary<EUpdateLevel, (ushort tMin, ushort tMax, ushort tCount)> LevelMap = new()
    {
        { EUpdateLevel.Level1, (70, 85, 200) },
        { EUpdateLevel.Level2, (85, 90, 100) },
        { EUpdateLevel.Level3, (85, 95, 50) },
    };
    /// <summary>
    /// 天气随机 buff 数组
    /// </summary>
    public const int WeatherRandomGainCount = 3;
    /// <summary>
    /// 刷新卡牌数组
    /// </summary>
    public const int CardGroupCount = 5;
    /// <summary>
    /// 英雄碎片合成数量
    /// </summary>
    public const int ResultantQuanatity = 3;
    /// <summary>
    /// 可刷新技能数量
    /// </summary>
    public const int CardSkillGroupCount = 2;
    /// <summary>
    /// 英雄技能可出现概率
    /// </summary>
    public const float AppearSkillCardProbability = 1.0f;

    #endregion

    #region 属性
    public static Camera MainCamera => Camera.main;
    public static float CurTime => Time.time;
    public static Transform PoolRoot => GameMgr.Ins.PoolRoot;
    public static Transform TestRoot => GameMgr.Ins.TestRoot;
    public static float TimeScale => Time.timeScale;
    public static float UpdateDeltaTime => Time.deltaTime;
    public static float FrameToMilliseconds => TimeScale / MaxFrameRate;
    #endregion


    #region 静态类
    public static AssetsMgr AssetsMgr => AssetsMgr.Ins;
    public static MathfMgr MathfMgr => MathfMgr.Ins;
    public static WorldMgr WorldMgr => WorldMgr.Ins;
    public static GizomMgr DrawGizom => GizomMgr.Ins;
    public static LifecycleMgr LifecycleMgr => LifecycleMgr.Ins;
    public static GainMgr GainMgr => GainMgr.Ins;
    public static WeaponMgr WeaponMgr => WeaponMgr.Ins;
    public static BuffMgr BuffMgr => BuffMgr.Ins;
    public static TableMgr TableMgr => TableMgr.Ins;
    public static TerrainMgr TerrainMgr => TerrainMgr.Ins;
    public static WeatherMgr WeatherMgr => WeatherMgr.Ins;
    public static WorldMapMgr WorldMapMgr => WorldMapMgr.Ins;
    public static PlayerMgr PlayerMgr => PlayerMgr.Ins;
    public static CameraMgr CameraMgr => CameraMgr.Ins;
    public static HeroIncubatorPoolMgr HeroIncubatorPoolMgr => HeroIncubatorPoolMgr.Ins;
    public static MonsterMgr MonsterMgr => MonsterMgr.Ins;
    public static UIMgr UIMgr => UIMgr.Ins;
    public static WorldWindowMgr WorldWindowMgr => WorldWindowMgr.Ins;
    public static EventSystemMgr EventSystemMgr => EventSystemMgr.Ins;
    public static GlobalEventMgr GlobalEventMgr => GlobalEventMgr.Ins;
    public static CardMgr CardMgr => CardMgr.Ins;
    public static HeroMgr HeroMgr => HeroMgr.Ins;
    public static EquipmentMgr EquipmentMgr => EquipmentMgr.Ins;
    public static UIWindowManager UIWindowManager => UIWindowManager.Ins;
    public static GameDataMgr GameDataMgr => GameDataMgr.Ins;
    public static LoadAssetManager LoadAssetManager => LoadAssetManager.Ins;
    public static CreateMapNew CreateMapNew => CreateMapNew.Ins;
    public static HeroCardPoolMgr HeroCardPoolMgr => HeroCardPoolMgr.Ins;
    public static AttackMgr AttackMgr => AttackMgr.Ins;
    public static FormationMgr FormationMgr => FormationMgr.Ins;
    public static AudioMgr AudioMgr => AudioMgr.Ins;
    #endregion

    #region Mono 静态类
    #endregion

    #region 功能函数
    public static bool RefIsNull(object f_Obj1)
    {
        return f_Obj1 == null;
    }
    /// <summary>
    /// 并行执行任务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="f_Tasks"></param>
    /// <param name="f_Callback"></param>
    /// <returns></returns>
    public static async UniTask ParallelTaskAsync<T>(List<T> f_Tasks, Func<T, UniTask> f_Callback)
    {
        UniTask[] tasks = new UniTask[f_Tasks.Count];
        for (int i = 0; i < f_Tasks.Count; i++)
        {
            var index = i;
            var item = f_Tasks[i];
            tasks[i] = UniTask.Create(async () =>
            {
                await UniTask.Delay((int)(UpdateDeltaTime * 1000) * index);
                await f_Callback.Invoke(item);
            });
        }
        await UniTask.WhenAll(tasks);
    }
    public static async UniTask ParallelTaskAsync<TKey, TValue>(Dictionary<TKey, TValue> f_Tasks, Func<TKey, TValue, UniTask> f_Callback)
    {
        UniTask[] tasks = new UniTask[f_Tasks.Count];
        var index = 0;
        foreach (var item in f_Tasks)
        {
            var tempItem = item;
            tasks[index++] = UniTask.Create(async () =>
            {
                await f_Callback.Invoke(tempItem.Key, tempItem.Value);
            });
        }
        await UniTask.WhenAll(tasks);
    }
    public static async UniTask ParallelTaskAsync(ushort f_Count, Func<ushort, UniTask> f_Callback)
    {
        UniTask[] tasks = new UniTask[f_Count];
        for (ushort i = 0; i < f_Count; i++)
        {
            var index = i;
            tasks[index] = UniTask.Create(async () =>
            {
                await f_Callback.Invoke(index);
            });
        }
        await UniTask.WhenAll(tasks);
    }
    public static async UniTask WaitSecondAsync(float f_Second)
    {
        await UniTask.Delay((int)(f_Second * 1000));
    }
    public static async UniTask<T> LoadTrailRenderAsync<T>(Vector3 f_StartPoint) where T : TrailRenderBase
    {
        var target = await GTools.AssetsMgr.LoadPrefabPoolAsync<T>(null, f_StartPoint, true);
        return target;
    }
    public static async UniTask UnLoadTrailRenderAsync<T>(T f_Target) where T : TrailRenderBase
    {
        await f_Target.WaitDestroyTimeAsync();
        await GTools.AssetsMgr.UnLoadPrefabPoolAsync<T>(f_Target);
    }
    public static async UniTask DoTweenAsync(float f_Time, Func<float, UniTask> f_Callback, Func<float, bool> f_Condition)
    {
        var doTweenID = $"{f_Callback.GetHashCode()}+{f_Condition.GetHashCode()}+{MathfMgr.GetRandomValue(0.0f, 1)}";
        await DOTween.To(() => 0.0f, async (value) =>
          {
              if (!f_Condition.Invoke(value))
              {
                  DOTween.Kill(doTweenID);
                  return;
              }
              await f_Callback.Invoke(value);
          }, 1.0f, f_Time)
            .SetEase(Ease.Linear)
            .SetId(doTweenID);
    }
    public static bool GetEntityActively(EntityData f_Entity)
    {
        return !RefIsNull(f_Entity);
    }
    #endregion


    public static async UniTask<bool> AddDicElementAsync<TKey, TValue>(Dictionary<TKey, TValue> f_Dic, TKey f_Key, Func<UniTask<TValue>> f_Value)
    {
        if (!f_Dic.ContainsKey(f_Key))
        {
            f_Dic.Add(f_Key, default(TValue));
            var value = await f_Value.Invoke();
            f_Dic[f_Key] = value;
            return true;
        }
        return false;
    }




    public static async void RunUniTask(UniTask f_Task)
    {
        await f_Task;
    }
    public static void RunUniTask(Action f_Task)
    {
        f_Task.Invoke();
    }

    public static bool UnityObjectIsVaild(UnityObjectData f_UnityObject)
    {
        var result = f_UnityObject != null && f_UnityObject.CurStatus != EPersonStatusType.Die;
        return result;
    }
    public static bool WorldObjectIsActive(WorldObjectBaseData f_UnityObject)
    {
        var result = f_UnityObject != null && f_UnityObject.GetObjBehaviorStatus();
        return result;
    }
    public static bool WorldObjectIsVaild(WorldObjectBaseData f_UnityObject)
    {
        if (!UnityObjectIsVaild(f_UnityObject))
        {
            return false;
        }
        if (!WorldObjectIsActive(f_UnityObject))
        {
            return false;
        }
        return true;
    }
    public static void FloaterHint(string f_Context)
    {
        Debug.Log(f_Context);
    }

    public static Vector3 GetMousePosToScene()
    {
        var viewPos = Input.mousePosition - new Vector3(Screen.width, Screen.height, 0) * 0.5f;
        var cameraPos = MainCamera.transform.position;

        var curPos = cameraPos + viewPos / 100;
        return curPos;
    }

}

public static class ExtendFunction
{
    public static void Add(this IncubatorAttributeInfo source, IncubatorAttributeInfo target)
    {
        source.BloodRatio += target.BloodRatio;
        source.HarmRatio += target.HarmRatio;
        source.DefenceRatio += target.DefenceRatio;
        source.AtkSpeedRatio += target.AtkSpeedRatio;
    }
    public static void Initialization(this UnityObjectData tran)
    {
        tran.SetLocalPosition(Vector3.zero);
        tran.SetLocalRotation(Vector3.zero);
        tran.SetLocalScale(Vector3.one);
    }

    public static bool ContainsEntityType(this WorldObjectBaseData f_WorldObjData, EEntityType f_EntityType)
    {
        var result = (f_WorldObjData.EntityType & f_EntityType) != 0;
        return result;
    }

    private const string SkillPath = "AnimationClip/";
    public static async UniTask<AnimationClip> LoadSkillAnimtionClip(this Entity_HeroBaseData f_HeroData, EPersonSkillType f_SkillType)
    {
        var skillPath = $"{SkillPath}{f_HeroData.HeroCradType}/{f_HeroData.HeroCradType}_{f_SkillType}";
        var clipAsset = await LoadAssetManager.Ins.LoadAsync<AnimationClip>(skillPath);
        if (clipAsset == null)
        {
            f_HeroData.LogError($"加载技能动画资源失败 {f_HeroData.HeroCradType} assetPath = {skillPath}");
        }
        return clipAsset;
    }

    public static bool MoveToChunk(this DependChunkData f_ependData, int f_ChunkIndex)
    {
        return GTools.CreateMapNew.MoveToChunk(f_ependData, f_ChunkIndex);
    }
    public static void AddChunkColor(this UnityObjectData f_ObjData, int f_Index, Color? f_Color = null)
    {
        var color = f_Color ?? Color.green;
        GTools.CreateMapNew.AddChunkColor(f_ObjData, f_Index, color);
    }
    public static void ClearChunkColor(this UnityObjectData f_ObjData, int f_Index)
    {
        GTools.CreateMapNew.ClearChunkColor(f_ObjData, f_Index);
    }
    public static void ShowAttackRange(this WorldObjectBaseData f_ObjData, Color? f_Color = null)
    {
        var index = f_ObjData.CurrentIndex;
        var atkRange = f_ObjData.CurAtkRange;
        var color = f_Color ?? Color.green;

        if (!GTools.CreateMapNew.TryGetRangeChunkByIndex(index, out var listIndex, null, false, atkRange))
        {
            return;
        }
        foreach (var item in listIndex)
        {
            f_ObjData.AddChunkColor(item, color);
        }
    }
    public static void HideAttackRange(this WorldObjectBaseData f_ObjData)
    {
        var index = f_ObjData.CurrentIndex;
        var atkRange = f_ObjData.CurAtkRange;

        if (!GTools.CreateMapNew.TryGetRangeChunkByIndex(index, out var listIndex, null, false, atkRange))
        {
            return;
        }
        foreach (var item in listIndex)
        {
            f_ObjData.ClearChunkColor(item);
        }
    }
    public static bool TryGetRandomNearTarget(this WorldObjectBaseData f_ObjData, out WorldObjectBaseData f_Target)
    {
        return GTools.CreateMapNew.TryGetRandomNearTarget(f_ObjData.CurrentIndex, f_ObjData.AttackLayerMask, f_ObjData.CurAtkRange, out f_Target);
    }
    public static bool TryGetRandomEnemy(this WorldObjectBaseData f_ObjData, out List<WorldObjectBaseData> f_ResultList)
    {
        return GTools.CreateMapNew.TryGetRandomTargetByWorldObjectType(f_ObjData.CurrentIndex, f_ObjData.AttackLayerMask, out f_ResultList, f_ObjData.CurAtkRange);
    }
    public static bool TryGetRandomTeam(this WorldObjectBaseData f_ObjData, out List<WorldObjectBaseData> f_ResultList, int f_Range)
    {
        return GTools.CreateMapNew.TryGetRandomTargetByWorldObjectType(f_ObjData.CurrentIndex, f_ObjData.LayerMask, out f_ResultList, f_Range);
    }
    public static bool TryGetRandomEnemy(this WorldObjectBaseData f_ObjData, int f_Range, out List<WorldObjectBaseData> f_ResultList)
    {
        return GTools.CreateMapNew.TryGetRandomTargetByWorldObjectType(f_ObjData.CurrentIndex, f_ObjData.AttackLayerMask, out f_ResultList, f_Range);
    }
    public static void EntityDamage(this WorldObjectBaseData f_ObjData, WorldObjectBaseData f_Target, int? f_Harm = null , int f_AddIsMagic = -1)
    {
        GTools.AttackMgr.EntityDamage(f_ObjData, f_Target, f_Harm ?? -f_ObjData.CurHarm, f_AddIsMagic);
    }
    public static bool TryGetNearTarget(this WorldObjectBaseData f_ObjData, EWorldObjectType f_ObjType, out WorldObjectBaseData f_Target)
    {
        f_Target = null;
        if (GTools.CreateMapNew.TryGetAllObject(f_ObjType, out var list))
        {
            var pathNodeCount = int.MaxValue;
            foreach (var item in list)
            {
                if (item is not WorldObjectBaseData objData)
                {
                    continue;
                }
                if (!GTools.UnityObjectIsVaild(objData))
                {
                    continue;
                }
                if (!GTools.WorldObjectIsActive(objData))
                {
                    continue;
                }
                if (!PathManager.Ins.TryGetAStarPath(f_ObjData.CurrentIndex, item.CurrentIndex, out var pathList, f_ObjData.ExtraCondition))
                {
                    continue;
                }
                if (pathNodeCount > pathList.Count)
                {
                    pathNodeCount = pathList.Count;
                    f_Target = objData;
                }
            }
        }
        return f_Target != null;
    }
    private static bool ExtraCondition(this WorldObjectBaseData f_ObjData, int f_ChunkIndex)
    {
        if (!GTools.CreateMapNew.TryGetChunkData(f_ChunkIndex, out var chunkData))
        {
            return false;
        }
        if (chunkData.IsPass() || chunkData.IsExistObj(f_ObjData.AttackLayerMask, out var _))
        {
            return true;
        }
        return false;
    }
    public static bool TryGetPath(this WorldObjectBaseData f_ObjData, int f_TargetIndex, out ListStack<PathElementData> f_ChunkList)
    {
        if (PathManager.Ins.TryGetAStarPath(f_ObjData.CurrentIndex, f_TargetIndex, out f_ChunkList, f_ObjData.ExtraCondition))
        {
            return true;
        }
        return false;
    }
    public static void InflictionGain(this WorldObjectBaseData f_ObjData, WorldObjectBaseData f_Recipient, EGainType f_GainType)
    {
        IGainUtil.InflictionGain(f_GainType, f_ObjData, f_Recipient);
    }
}