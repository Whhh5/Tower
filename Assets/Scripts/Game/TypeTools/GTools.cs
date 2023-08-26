using B1;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

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
    public static WorldMapMgr WorldMapMgr  => WorldMapMgr.Ins; 
    public static PlayerMgr PlayerMgr => PlayerMgr.Ins; 
    public static CameraMgr CameraMgr => CameraMgr.Ins;
    public static HeroIncubatorPoolMgr HeroIncubatorPoolMgr => HeroIncubatorPoolMgr.Ins;
    public static MonsterMgr MonsterMgr => MonsterMgr.Ins;
    public static UIMgr UIMgr => UIMgr.Ins;
    public static WorldWindowMgr WorldWindowMgr => WorldWindowMgr.Ins;
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

    public static bool UnityObjectIsActive(UnityObjectData f_UnityObject)
    {
        var result = f_UnityObject != null && f_UnityObject.CurStatus != EPersonStatusType.Die;
        return result;
    }

    public static void FloaterHint(string f_Context)
    {
        Debug.Log(f_Context);
    }
}
