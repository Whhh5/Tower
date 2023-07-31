using System.Collections;
using System.Collections.Generic;
using B1;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UILobbyPageData : UIWindowPageData
{
    public string pName = "";
    public int pIndex = 0;
    public int pIconID = 0;
}
public class UILobbyPage : UIWindowPage
{
    // ��̬����ģ��
    public static UILobbyPageData pPageData = null;
    public static async UniTask SetPageData(UILobbyPageData f_UILobbyPageData)
    {
        pPageData = f_UILobbyPageData;
        await UniTask.Delay(0);
    }
    // �ڲ�����ģ��
    public string pNmae = "UILobbyPage";

    // ע��ͼ��
    protected override EAssetName SpriteAltas => EAssetName.UILobbySpriteAltas;
    // ע�ᴰ��
    protected override List<EAssetName> GetWindowNameAsync()
    {
        var windowList = new List<EAssetName>()
        {
            EAssetName.UILobby,
            EAssetName.UILobbyItemInfo,
            EAssetName.UILobbyNavigationBar,
        };
        return windowList;
    }
}
