using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class Scroll<TItem, TItemData> : MonoBehaviour
    where TItem: ScrollItem<TItemData>
    where TItemData : ScrollItemData
{
    /* 
    * ---------------================---------------
    * ------======------ 自身变量引用
    * ---------------================---------------
    */
    public RectTransform Rect => GetComponent<RectTransform>();
    private RectTransform m_Content = null;

    /* 
    * ------======------ 数据相关
    */
    [SerializeField, Tooltip("目标 item")]
    protected TItem m_Item = null;
    [SerializeField, Tooltip("当前显示的数据索引范围")]
    protected Vector2Int m_CurRange = Vector2Int.zero;
    [SerializeField, Tooltip("存放所有数据")]
    protected ListStack<TItemData> mDic_ItemData = new("Scroll List", 30);


    /* 
    * ------======------ item 实例相关
    */
    [SerializeField, Tooltip("存放所有 item")]
    protected Dictionary<ushort, (TItem tItem, TItemData tData)> mDic_Item = new();
    [SerializeField, Tooltip("当前视图最大显示的数量")]
    protected ushort m_ViewMaxCount = 0;
    [SerializeField, Tooltip("最大数量偏移量")]
    protected ushort m_CountOffset = 0;
    // 当前显示的最大数量
    public ushort ViewMaxCount => (ushort)(m_ViewMaxCount + m_CountOffset);

    /* 
    * ------======------ 功能配置
    */
    [SerializeField, Tooltip("自适应大小")]
    protected bool m_IsAutoItemSize = false;

    /* 
    * ------======------ 辅助变量
    */






    /* 
    * ---------------================---------------
    * ------======------ 数据配置
    * ---------------================---------------
    */
    public void Initialization()
    {
        UpdateMaxCount();
        InitItem();
    }
    public void Release()
    {

    }
    // 计算最大数量数据
    public void UpdateMaxCount()
    {
        var sizeDelta = Rect.sizeDelta;

    }
    // 计算 item 大小
    public void UpdateItemSize(TItemData f_Item)
    {
        var cullingGroup = new CullingGroup();
        Vector2 minPos = Vector2.zero;
        Vector2 maxPos = Vector2.zero;
        //foreach (RectTransform item in f_Item.transform)
        //{
            
        //}
    }
    // 初始化 item 数量
    public void InitItem()
    {

    }
    // 设置所有数据引用, 这将会清空之前所有的数据
    public void SetData(List<TItemData> f_Data)
    {
        for (ushort i = 0; i < f_Data.Count; i++)
        {
            var item = f_Data[i];
            mDic_ItemData.Push(item);
        }

    }
    /* 
    * ---------------================---------------
    * ------======------ 数据更新
    * ---------------================---------------
    */
    // 更新索引
    public void SetIndex(TItemData f_Item, ushort f_ToIndex)
    {
        var oldIndex = f_Item.Index;
        f_Item.SetIndex(f_ToIndex);
        f_Item.UpdateIndexClick(oldIndex, f_ToIndex);
    }
    // 刷新所有 item
    public void UpdateAllItem()
    {

    }
    // 更新一个 item 上面的数据
    public void UpdateItemOne(TItemData f_ItemData)
    {
        
    }
    // 删除一个 item
    public void RemoveItemOne(TItemData f_ItemData)
    {
        if (mDic_ItemData.Contains(f_ItemData))
        {
            mDic_ItemData.Remove2(f_ItemData, (target, oldIndex, newIndex)=>
            {
                SetIndex(target, (ushort)newIndex);
            });
        }
    }
    // 在末尾添加一个 item
    public void AddItemOne(TItemData f_ItemData)
    {

    }
    // 从末尾删除一个 item
    public void RemoveItem()
    {

    }
    // 插入一个数据
    public void InsertItem(ushort f_Index, TItem f_Item)
    {

    }


}
