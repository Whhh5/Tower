using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScrollItemData
{    
    /* 
    * ---------------================---------------
    * ------======------ 共用变量
    * ---------------================---------------
    */
    // 标识改数据的唯一索引
    public ushort ID = 0;
    // 在整个列表中的第几个
    public ushort Index { get; private set; } = 0;
    public ScrollItemData(ushort f_ID)
    {
        InitParams(f_ID);
    }
    public void InitParams(ushort f_ID)
    {
        ID = f_ID;
    }    
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
        ID = 0;
    }
    // 数据刷新调用
    public void Initialization(ushort f_FlagIndex)
    {
        ID = f_FlagIndex;
    }    
    /* 
    * ---------------================---------------
    * ------======------ Scroll 调用
    * ---------------================---------------
    */
    // 设置列表内部的索引
    public void SetIndex(ushort f_Index)
    {
        Index = f_Index;
    }




    // 更新列表内部的索引
    public abstract void UpdateIndexClick(ushort f_OldIndex, ushort f_NewOndex);
    // 每次加载资源调用
    public abstract void LoadAssets();
    // 每次释放资源调用
    public abstract void ReleaseAssets();
}
