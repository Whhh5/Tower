using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

public class UIGameLevel : UIWindow
{
    [SerializeField]
    private Transform m_LevelListItem = null;
    private Dictionary<EMapLevelType, Transform> m_LevelList = new();
    public override async UniTask AwakeAsync()
    {
        m_LevelListItem.gameObject.SetActive(false);

        for (int i = 0; i < (int)EMapLevelType.EnumCount; i++)
        {
            var level = (EMapLevelType)i;
            if (m_LevelList.ContainsKey(level))
            {
                continue;
            }
            var obj = GameObject.Instantiate(m_LevelListItem, m_LevelListItem.parent);

            obj.GetChildCom<TextMeshProUGUI>(EChildName.Txt_Level).SetText(i.ToString());
            if (GTools.GameDataMgr.TryGetLevelData(level, out var leveldata))
            {
                obj.GetChildCom<Transform>(EChildName.Tran_Icon1).gameObject.SetActive(leveldata.IsPass);
            }

            obj.GetChildCom<Button>(EChildName.Btn_Click).onClick.AddListener(async () =>
            {
                GameDataMgr.Ins.SetMapData(level);
                await GTools.UIWindowManager.UnloadWindowAsync(this);
            });

            obj.gameObject.SetActive(true);
            m_LevelList.Add(level, obj);
        }
    }

    public override async UniTask OnShowAsync()
    {
        
    }

    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();

        foreach (var item in m_LevelList)
        {
            GameObject.Destroy(item.Value.gameObject);
        }
        m_LevelList.Clear();
    }

}
