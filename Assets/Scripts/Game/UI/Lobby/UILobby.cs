using System.Collections;
using System.Collections.Generic;
using B1.UI;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using B1;
using UnityEngine.AddressableAssets;

public class UILobby : UIWindow
{
    public UILobbyPage CurPage => GetPage<UILobbyPage>();


    public Button m_Button;
    public ESpriteName m_SpriteName;
    public Image m_TestImage = null;

    public Button m_ChangeImage1 = null;
    public Button m_ChangeImage2 = null;


    public override async UniTask AwakeAsync()
    {
        await DelayAsync();
        m_Button.onClick.AddListener(OnClickAsync);


        m_ChangeImage1.onClick.AddListener(async () =>
        {
            var result = await CurPage.LoadSpriteAsync(ESpriteName.Caiji_cj_shougetubiao);
            if (result.tResult)
            {
                m_TestImage.sprite = result.tSprite;
            }
        });
        m_ChangeImage2.onClick.AddListener(async () =>
        {
            var result = await CurPage.LoadSpriteAsync(ESpriteName.Caiji_cj_wakuangtubiao);
            if (result.tResult)
            {
                m_TestImage.sprite = result.tSprite;
            }
        });
    }

    public override async UniTask OnShowAsync()
    {
        await DelayAsync();

    }
    public override async UniTask ShowAsync()
    {
        await base.ShowAsync();

        Log($"{CurPage.pNmae}");
    }

    public async void OnClickAsync()
    {
        await CurPage.ShowStackAsync(EAssetName.UILobbyItemInfo);

    }
}
