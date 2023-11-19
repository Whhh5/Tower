using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAssetName : int
{
    None = int.MinValue,

    #region 预制体
    #region UILogin
    UILogin,
    #endregion

    #region UILobby
    UILobby,
    UILobbyItemInfo,
    UILobbyNavigationBar,
    #endregion

    #region UIMap
    UIMap,
    #endregion

    #region UISystem
    UGUISystem,
    UIGameStart,
    UINavigationBar,
    UIGameLevel,
    UIHeroCardSelect,
    UIGameMonster,
    UIGameFinish,
    UIFormationInfo,
    UIDrawWindow,
    #endregion

    #region UIAppPlane
    UIAppPlane,
    #endregion


    #region WorldWindow
    UIEntityBloodWindow,
    #endregion

    UITest1,


    #region MyRegion
    WorldDamageText,
    GameManager,
    Emitter_GuidedMissileBaseCommon,
    EmitterElement_GuidedMissile,
    Person_Enemty,
    Person_Enemy_Crab,
    Person_Player,
    TrailRender_Common,
    Emitter_SwordLow,
    EmitterElement_SwordLow,
    Emitter_SwordHeight,
    WorldBuffHint,
    #endregion





    #endregion



    #region 图集

    UILobbySpriteAltas,


    #endregion



    #region Script Object

    


    #endregion

    EnumCount,
}
public enum EAssetLable
{
    Prefab,
    Sprite,
    spriteAtlas,
    EnumCount,
}
public interface IOnDestroyAsync
{
    /// <summary>
    /// 当对象被加载出来首先被调用
    /// </summary>
    /// <returns></returns>
    UniTask OnLoadAsync();
    /// <summary>
    /// 当对象被卸载调用
    /// </summary>
    /// <returns></returns>
    UniTask OnUnLoadAsync();
}
public enum EUpdateLevel : byte
{
    Level1,
    Level2,
    Level3,
    EnumCount,
}
[System.Flags]
public enum ELayer : int
{
    Default = 1,
    TransparentFX = 1 << 1,
    IgnoreRatCast = 1 << 2,
    ___1 = 1 << 3,
    Water = 1 << 4,
    UI = 1 << 5,
    Terrain = 1 << 6,
    Enemy = 1 << 7,
    Player = 1 << 8,
    FlyingProp = 1 << 9,
    Tower = 1 << 10,
    All = int.MaxValue,
}
public enum ETowerStatus
{
    Idle,
    Start,
    Stop,
    Destriy,
}

#region 方法返回值结果
public enum EResult
{
    Succeed,
    Defeated,
}
public class ResultData<T>
{
    public ResultData(EResult f_result = EResult.Defeated)
    {
        Initialization(f_result);
    }
    public EResult Result { get; private set; }
    public T Value { get; private set; }
    public void Initialization(EResult f_result)
    {
        Result = f_result;
        Value = default(T);
    }
    public void SetData(T f_Value, EResult f_result = EResult.Succeed)
    {
        Result = f_result;
        Value = f_Value;
    }
}
#endregion

// interface
public interface IUpdateBase
{
    public int UpdateLevelID { get; set; }
    public EUpdateLevel UpdateLevel { get; }
    public float LasteUpdateTime { get; set; }
    public float UpdateDelta { get; set; }
    public void OnUpdate();
}


public interface IEntityDestry
{
    /// <summary>
    /// 当前实体依赖的实体被销毁时调用
    /// </summary>
    /// <returns></returns>
    UniTask CurEntityDestroyAsync();
}