using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkillInfoData : UnityObjectData
{
    public UISkillInfoData() : base(0)
    {
    }
    public override EAssetKey AssetPrefabID => EAssetKey.UISkillInfo;
    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    public Entity_HeroBaseData TargetObject = null;
    public List<UIItem_SkillData> SkillItemList = new();
    public int CurSkillIndex = 0;
    public UISkillInfo UISkillInfo => GetCom<UISkillInfo>();

    public void Initialization(Entity_HeroBaseData f_TargetObject)
    {
        TargetObject = f_TargetObject;

        if (GTools.TableMgr.TryGetHeroCradInfo(f_TargetObject.HeroCradType, out var heroInfo))
        {
            var skillCount = heroInfo.SkillLinkInfos.Count;
            for (int i = 0; i < skillCount; i++)
            {
                var skillItem = new UIItem_SkillData();
                skillItem.ListIndex = i;
                skillItem.SetName(i.ToString());
                SkillItemList.Add(skillItem);
            }
        }
    }

    public override void AfterLoad()
    {
        base.AfterLoad();

        UISkillInfo.Initialization();
    }

    public void AddSkill(EPersonSkillType f_SkillType)
    {
        if (GTools.TableMgr.TryGetPersonSkillInfo(f_SkillType, out var skillInfo))
        {
            SkillItemList[CurSkillIndex++].UpdateItemData(skillInfo, null);
            if (TargetObject.TryGetCurCanSkillCount(out var num))
            {
                var startIndex = SkillItemList.Count;
                for (int i = startIndex - 1; i >= startIndex - num; i--)
                {
                    SkillItemList[i].SetLockStatus(ELockStatus.Lock);
                }
            }
        }
    }

    public override bool IsUpdateEnable => true;
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (GTools.UnityObjectIsVaild(TargetObject))
        {
            var uguiPos = IUIUtil.GetUGUIPosByWorld(TargetObject.PointUp + new Vector3(0, 2.0f, 0), true);
            var worldPos = IUIUtil.GetWorldPosBuyUGUIPos(uguiPos);
            SetPosition(worldPos);
        }
    }
}
public class UISkillInfo : ObjectPoolBase
{
    [SerializeField]
    private RectTransform m_ItemRoot = null;
    public UISkillInfoData Data => GetData<UISkillInfoData>();

    public async void Initialization()
    {
        var curChildCount = m_ItemRoot.childCount;
        for (int i = 0; i < Data.SkillItemList.Count; i++)
        {
            var item = Data.SkillItemList[i];

            item.Initialization();
            item.UpdateItemData(null, m_ItemRoot);
        }
        await UniTask.Delay(2000);
        for (int i = 0; i < Data.SkillItemList.Count; i++)
        {
            var item = Data.SkillItemList[i];
            item.PrefabTarget.transform.SetSiblingIndex(i + curChildCount);
        }
    }
}
