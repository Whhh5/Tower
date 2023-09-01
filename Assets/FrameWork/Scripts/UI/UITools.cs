using System.Collections;
using System.Collections.Generic;
using B1;
using Cysharp.Threading.Tasks;
using UnityEngine;


public enum EUIAppRoot : ushort
{
    None,
    Scene,
    App1,
    App2,
    App3,
    System,
    EnumCount,
}
[SerializeField]

public enum ESpriteName
{
    Caiji_cj_famutubiao,
    Caiji_cj_shougetubiao,
    Caiji_cj_wakuangtubiao,
    Caiji_cj_zhipitubiao,
    EnumCount,
}
public enum EUIWindowPage : int
{
    None,
    UILobbyPage,
    UITestPage1,
    UILoginPage,
    UIAppPlanePage,
    UIMapPage,
    EnumCount,
}
public enum EScrollViewListItem
{
    EnumCount,
}
public interface IUIWindowPage
{

}





public interface IAppRoot
{
    EUIAppRoot AppRoot { get; }
}
public enum EUIElementName
{
    None,


    Tex_Name,
    Tex_Dec,
    Tex_Num,


    Img_Icon,
    Img_Bg,

    Btn_Close,
    Btn_Play,
    Btn_Change,

}


// ui ³ØÎïÌå¸¸Àà
public abstract class UIObjectPoolBase : MonoBase, IObjectPoolBase
{
    public bool UpdateInteractable = false;
    public float UpdateDelta { get; set; }
    public float LasteUpdateTime { get; set; }
    public int UpdateLevelID { get; set; }
    public string PoolKey { get; set; }
    public int AssetName { get; set; }
    public int AssetLable { get; set; }
    public bool IsActively { get; set; }

    public EAssetName EAssetName => (EAssetName)AssetName;
    public EAssetLable EAssetLable => (EAssetLable)AssetLable;

    public EUpdateLevel UpdateLevel { get => GTools.UIWindowUpdateLevel; set => value = value; }

    public abstract UniTask OnUnLoadAsync();

    public abstract UniTask OnLoadAsync();

    public abstract UniTask OnStart(params object[] f_Params);

    public abstract void OnUpdate();
}