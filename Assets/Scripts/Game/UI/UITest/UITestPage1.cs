using B1.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITestPage1 : UIWindowPage
{
    protected override EAssetName SpriteAltas => EAssetName.UILobbySpriteAltas;

    protected override List<EAssetName> GetWindowNameAsync()
    {
        return new List<EAssetName>()
            {
                EAssetName.UITest1,
            };
    }
}
