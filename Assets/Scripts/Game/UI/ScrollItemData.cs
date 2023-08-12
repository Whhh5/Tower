using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScrollItemData
{    
    /* 
    * ---------------================---------------
    * ------======------ ���ñ���
    * ---------------================---------------
    */
    // ��ʶ�����ݵ�Ψһ����
    public ushort ID = 0;
    // �������б��еĵڼ���
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
        ID = 0;
    }
    // ����ˢ�µ���
    public void Initialization(ushort f_FlagIndex)
    {
        ID = f_FlagIndex;
    }    
    /* 
    * ---------------================---------------
    * ------======------ Scroll ����
    * ---------------================---------------
    */
    // �����б��ڲ�������
    public void SetIndex(ushort f_Index)
    {
        Index = f_Index;
    }




    // �����б��ڲ�������
    public abstract void UpdateIndexClick(ushort f_OldIndex, ushort f_NewOndex);
    // ÿ�μ�����Դ����
    public abstract void LoadAssets();
    // ÿ���ͷ���Դ����
    public abstract void ReleaseAssets();
}
