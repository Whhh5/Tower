using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ScrollItemDataTest : ScrollItemData
{
    public ScrollItemDataTest(ushort f_ID) : base(f_ID)
    {

    }

    public override void LoadAssets()
    {
        
    }

    public override void ReleaseAssets()
    {

    }

    public override void UpdateIndexClick(ushort f_OldIndex, ushort f_NewOndex)
    {
        
    }
}
public class ScrollItemTest : ScrollItem<ScrollItemDataTest>
{
    public Image m_TargetImage = null;
    public TextMeshProUGUI m_TextMeshPro = null;

    public override void LoadAssets()
    {
        m_TargetImage.sprite = LoadSpriteAsync("");
    }
    public override void UpdateAssets()
    {
        
    }

    public override void ReleaseAssets()
    {
        
    }

}
