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
    * ------======------ ��ʼ������
    * ---------------================---------------
    */
    // ʵ������ɵ���
    public void LoadAsync()
    {

    }
    // destroy ����
    public void UnLoadAsync()
    {
        ReleaseAssets();
    }
    // ����ˢ�µ���
    public void Initialization(TItemData f_ItemData)
    {
        SetData(f_ItemData);
    }
    // ��ʼ������
    public void SetData(TItemData f_ItemData)
    {
        if (f_ItemData == m_ItemData) return;
        ReleaseAssets();
        LoadAssets();
    }




    /* 
    * ---------------================---------------
    * ------======------ ��Դ����
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
    * ------======------ Scroll ����
    * ---------------================---------------
    */
    // ÿ�μ�����Դ����
    public abstract void LoadAssets();
    // ÿ��ˢ����Դ����
    public abstract void UpdateAssets();
    // ÿ���ͷ���Դ����
    public abstract void ReleaseAssets();


}
