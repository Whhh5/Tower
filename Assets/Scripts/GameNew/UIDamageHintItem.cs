using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDamageHintItemData : UnityObjectData
{
    public UIDamageHintItemData() : base(0)
    {
    }
    public override EAssetKey AssetPrefabID => EAssetKey.UIDamageHintItem;
    public override EWorldObjectType ObjectType => EWorldObjectType.None;
    private UIDamageHintItem EntityTarget => GetCom<UIDamageHintItem>();

    public Vector3 BodyPos = Vector3.zero;
    public float Alpha = 0.0f;
    public int FontSize = 0;
    public string TMPContent;
    public Color MainColor  = Color.white;


    public override async void AfterLoad()
    {
        base.AfterLoad();

        var startPos1 = new Vector3(GTools.MathfMgr.GetRandomValue(-40, 40), GTools.MathfMgr.GetRandomValue(-40, 40), 0);
        var toPos1 = startPos1 + new Vector3(GTools.MathfMgr.GetRandomValue(-10, 10), GTools.MathfMgr.GetRandomValue(10, 20), 0);
        var toPos2 = toPos1 + new Vector3(GTools.MathfMgr.GetRandomValue(-10, 10), GTools.MathfMgr.GetRandomValue(20, 40), 0);

        await DOTween.To(() => 0.0f, slider =>
          {
              SetMainAlpha(slider);
              var pos = Vector3.Lerp(startPos1, toPos1, slider);
              SetBodyPosition(pos);

          }, 1.0f, 0.2f);
        await UniTask.Delay(FontSize * 5);
        await DOTween.To(() => 0.0f, slider =>
        {
            SetMainAlpha(1 - slider);
            var pos = Vector3.Lerp(toPos1, toPos2, slider);
            SetBodyPosition(pos);

        }, 1.0f, 0.1f);

        ILoadPrefabAsync.UnLoad(this);
    }


    public void SetBodyPosition(Vector3 f_Pos)
    {
        BodyPos = f_Pos;
        if (EntityTarget != null)
        {
            EntityTarget.SetBodyPosition();
        }
    }
    public void SetMainAlpha(float f_Alpha)
    {
        Alpha = f_Alpha;
        if (EntityTarget != null)
        {
            EntityTarget.SetMainAlpha();
        }
    }
    public void SetFontSize(int f_FontSize)
    {
        FontSize = f_FontSize;
        if (EntityTarget != null)
        {
            EntityTarget.SetFontSize();
        }
    }
    public void SetTMPText(string f_Content)
    {
        TMPContent = f_Content;
        if (EntityTarget != null)
        {
            EntityTarget.SetTMPText();
        }
    }
    public void SetFontColor(Color f_Color)
    {
        MainColor = f_Color;
        if (EntityTarget != null)
        {
            EntityTarget.SetFontColor();
        }
    }

}
public class UIDamageHintItem : ObjectPoolBase
{
    [SerializeField]
    private RectTransform m_Body = null;
    [SerializeField]
    private CanvasGroup m_CanvasGroup = null;
    [SerializeField]
    private TextMeshProUGUI m_MainText = null;
    private UIDamageHintItemData EntityData => GetData<UIDamageHintItemData>();

    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        await base.OnStartAsync(f_Params);

        SetBodyPosition();
        SetMainAlpha();
        SetFontSize();
        SetTMPText();
        SetFontColor();
    }
    public void SetBodyPosition()
    {
        m_Body.anchoredPosition3D = EntityData.BodyPos;
    }
    public void SetMainAlpha()
    {
        m_CanvasGroup.alpha = EntityData.Alpha;
    }
    public void SetFontSize()
    {
        m_MainText.fontSize = EntityData.FontSize;
    }
    public void SetTMPText()
    {
        m_MainText.text = EntityData.TMPContent;
    }
    public void SetFontColor()
    {
        m_MainText.color = EntityData.MainColor;
    }
}
