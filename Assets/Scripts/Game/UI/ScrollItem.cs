using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScrollItem<TItemData> : MonoBehaviour
    where TItemData: ScrollItemData
{
    public RectTransform Rect => GetComponent<RectTransform>();

    private List<uint> mList_ImageHandleID = new();
    protected TItemData m_ItemData = null;

    /* 
    * ---------------================---------------
    * ------======------ 初始化数据
    * ---------------================---------------
    */
    // 实例化完成调用
    public void LoadAsync()
    {

    }
    // destroy 调用
    public void UnLoadAsync()
    {
        ReleaseAssets();
    }
    // 数据刷新调用
    public void Initialization(TItemData f_ItemData)
    {
        SetData(f_ItemData);
    }
    // 初始化数据
    public void SetData(TItemData f_ItemData)
    {
        if (f_ItemData == m_ItemData) return;
        ReleaseAssets();
        LoadAssets();
    }




    /* 
    * ---------------================---------------
    * ------======------ 资源加载
    * ---------------================---------------
    */
    private Dictionary<string, ushort> mDic_Assets = new();
    public Sprite LoadSpriteAsync(string f_Path)
    {

        return null;
    }
    public void UnLoadSpriteAsync(uint HandleID)
    {




    }    
    /* 
    * ---------------================---------------
    * ------======------ Scroll 调用
    * ---------------================---------------
    */
    // 每次加载资源调用
    public abstract void LoadAssets();
    // 每次刷新资源调用
    public abstract void UpdateAssets();
    // 每次释放资源调用
    public abstract void ReleaseAssets();


}
