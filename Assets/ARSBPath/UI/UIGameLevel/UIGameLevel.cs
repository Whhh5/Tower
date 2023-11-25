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
    [SerializeField]
    private Button m_ReturnBtn = null;
    private Dictionary<EMapLevelType, Transform> m_LevelList = new();
    public override async UniTask AwakeAsync()
    {
        GTools.AudioMgr.PlayBackground(EAudioType.Scene_SelectLevel);
        m_LevelListItem.gameObject.SetActive(false);
        m_ReturnBtn.onClick.RemoveAllListeners();
        m_ReturnBtn.onClick.AddListener(async () =>
        {
            await GTools.UIWindowManager.UnloadWindowAsync(this);
            await GTools.UIWindowManager.LoadWindowAsync<UIGameStart>(EAssetName.UIGameStart);
        });

        for (int i = 0; i < (int)EMapLevelType.EnumCount; i++)
        {
            var level = (EMapLevelType)i;
            if (m_LevelList.ContainsKey(level))
            {
                continue;
            }
            var isLock = false;
            if (i == 0)
            {

            }
            else if (GTools.GameDataMgr.TryGetLevelData(level - 1, out var leveldata))
            {
                isLock = !leveldata.IsPass;
            }
            else
            {
                continue;
            }
            var obj = GameObject.Instantiate(m_LevelListItem, m_LevelListItem.parent);

            obj.GetChildCom<TextMeshProUGUI>(EChildName.Txt_Level).SetText(i.ToString());
            obj.GetChildCom<Transform>(EChildName.Tran_Icon1).gameObject.SetActive(isLock);

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
