using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class UIGameStart : UIWindow
{
    [SerializeField]
    private Button m_GameStart = null;
    public override async UniTask AwakeAsync()
    {
        m_GameStart.onClick.AddListener(async () =>
        {
            await GTools.UIWindowManager.LoadWindowAsync<UIGameLevel>(EAssetName.UIGameLevel);
            await GTools.UIWindowManager.UnloadWindowAsync(this);
        });
    }

    public override async UniTask OnShowAsync()
    {
        
    }
}
