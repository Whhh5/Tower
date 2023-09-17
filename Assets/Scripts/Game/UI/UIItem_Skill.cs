using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem_SkillData : UIItemBaseData<PersonSkillInfo>
{
    public override AssetKey AssetPrefabID => AssetKey.UIItem_Skill;
    public UIItem_Skill UIItem_Skill => GetCom<UIItem_Skill>();
    public ELockStatus LockStatus = ELockStatus.UnLock;
    public override void UpdateItemData(PersonSkillInfo f_Data, Transform f_Parent)
    {
        base.UpdateItemData(f_Data, f_Parent);
        if (f_Data == null)
        {
            return;
        }
        UpdateItemData2();
    }
    public void SetLockStatus(ELockStatus f_IsLock)
    {
        LockStatus = f_IsLock;
        UpdateItemData2();
    }
    public override void AfterLoad()
    {
        base.AfterLoad();

        if (Data == null)
        {
            UIItem_Skill.HideImage();
            return;
        }
        UpdateItemData2();
    }
    public void UpdateItemData2()
    {
        if (UIItem_Skill != null)
        {
            UIItem_Skill.UpdateItemData();
        }
    }
}
public class UIItem_Skill : UIItemBase
{
    [SerializeField]
    private Image m_IconImg = null;
    [SerializeField]
    private Image m_QualityImg = null;
    [SerializeField]
    private GameObject m_LockObj = null;
    public UIItem_SkillData Data => GetData<UIItem_SkillData>();
    public async void UpdateItemData()
    {
        HideImage();
        if (Data.LockStatus == ELockStatus.Lock)
        {
            m_LockObj.SetActive(true);
            return;
        }
        if (GTools.TableMgr.TryGetAssetPath(Data.Data.IconKey, out var path))
        {
            var sprite = await ILoadSpriteAsync.LoadAsync(path);
            m_IconImg.gameObject.SetActive(true);
            m_IconImg.sprite = sprite;
        }
        if (GTools.TableMgr.TryGetQualityInfo(Data.Data.Quality, out var qualityInfo))
        {
            m_IconImg.color = qualityInfo.Color;
            m_QualityImg.gameObject.SetActive(true);
        }
        
    }
    public void HideImage()
    {
        m_IconImg.gameObject.SetActive(false);
        m_QualityImg.gameObject.SetActive(false);
        m_LockObj.SetActive(false);
    }
}
