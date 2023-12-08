using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using B1.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using System;

public class UISettingWindow : UIWindow
{
    private class SettingData
    {
        public string name;
        public Action click;
        public RectTransform item;
        public void InitData(RectTransform f_TargetItem)
        {
            var itemObj = GameObject.Instantiate(f_TargetItem, f_TargetItem.parent);
            var btn = itemObj.GetChildCom<Button>(EChildName.Btn_Click);
            var title = itemObj.GetChildCom<TextMeshProUGUI>(EChildName.Txt_Title);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => click());
            title.text = name;
            itemObj.gameObject.SetActive(true);
            item = itemObj;
        }
        public void Destroy()
        {
            GameObject.Destroy(item.gameObject);
        }
    }

    private Dictionary<int, SettingData> m_SettingFuncList = new()
    {
        {
            1,
            new()
            {
                name = "返回",
                click = () =>
                {
                    GameManager.ReturnSelectWindow();
                },
                item = null,
            }
        },

    };
    [SerializeField]
    private Button m_SettingCloseBtn = null;
    [SerializeField]
    private RectTransform m_SettingBtnItem = null;

    public override async UniTask AwakeAsync()
    {
        GameManager.SetGameScale(0);
        // 初始化设置列表
        m_SettingBtnItem.gameObject.SetActive(false);
        foreach (var item in m_SettingFuncList)
        {
            item.Value.InitData(m_SettingBtnItem);
        }


        m_SettingCloseBtn.onClick.RemoveAllListeners();
        m_SettingCloseBtn.onClick.AddListener(async () =>
        {
            await GTools.UIWindowManager.UnloadWindowAsync(this);
        });
    }

    public override async UniTask OnShowAsync()
    {

    }

    public override async UniTask OnUnLoadAsync()
    {
        foreach (var item in m_SettingFuncList)
        {
            item.Value.Destroy();
        }
        await base.OnUnLoadAsync();
        GameManager.SetGameScale(1);
    }


}
