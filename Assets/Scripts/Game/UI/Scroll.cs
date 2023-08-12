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
    * ------======------ �����������
    * ---------------================---------------
    */
    public RectTransform Rect => GetComponent<RectTransform>();
    private RectTransform m_Content = null;

    /* 
    * ------======------ �������
    */
    [SerializeField, Tooltip("Ŀ�� item")]
    protected TItem m_Item = null;
    [SerializeField, Tooltip("��ǰ��ʾ������������Χ")]
    protected Vector2Int m_CurRange = Vector2Int.zero;
    [SerializeField, Tooltip("�����������")]
    protected ListStack<TItemData> mDic_ItemData = new("Scroll List", 30);


    /* 
    * ------======------ item ʵ�����
    */
    [SerializeField, Tooltip("������� item")]
    protected Dictionary<ushort, (TItem tItem, TItemData tData)> mDic_Item = new();
    [SerializeField, Tooltip("��ǰ��ͼ�����ʾ������")]
    protected ushort m_ViewMaxCount = 0;
    [SerializeField, Tooltip("�������ƫ����")]
    protected ushort m_CountOffset = 0;
    // ��ǰ��ʾ���������
    public ushort ViewMaxCount => (ushort)(m_ViewMaxCount + m_CountOffset);

    /* 
    * ------======------ ��������
    */
    [SerializeField, Tooltip("����Ӧ��С")]
    protected bool m_IsAutoItemSize = false;

    /* 
    * ------======------ ��������
    */






    /* 
    * ---------------================---------------
    * ------======------ ��������
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
    // ���������������
    public void UpdateMaxCount()
    {
        var sizeDelta = Rect.sizeDelta;

    }
    // ���� item ��С
    public void UpdateItemSize(TItemData f_Item)
    {
        var cullingGroup = new CullingGroup();
        Vector2 minPos = Vector2.zero;
        Vector2 maxPos = Vector2.zero;
        //foreach (RectTransform item in f_Item.transform)
        //{
            
        //}
    }
    // ��ʼ�� item ����
    public void InitItem()
    {

    }
    // ����������������, �⽫�����֮ǰ���е�����
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
    * ------======------ ���ݸ���
    * ---------------================---------------
    */
    // ��������
    public void SetIndex(TItemData f_Item, ushort f_ToIndex)
    {
        var oldIndex = f_Item.Index;
        f_Item.SetIndex(f_ToIndex);
        f_Item.UpdateIndexClick(oldIndex, f_ToIndex);
    }
    // ˢ������ item
    public void UpdateAllItem()
    {

    }
    // ����һ�� item ���������
    public void UpdateItemOne(TItemData f_ItemData)
    {
        
    }
    // ɾ��һ�� item
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
    // ��ĩβ���һ�� item
    public void AddItemOne(TItemData f_ItemData)
    {

    }
    // ��ĩβɾ��һ�� item
    public void RemoveItem()
    {

    }
    // ����һ������
    public void InsertItem(ushort f_Index, TItem f_Item)
    {

    }


}
