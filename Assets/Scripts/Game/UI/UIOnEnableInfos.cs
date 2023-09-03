using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOnEnableInfosShowData
{
    public AssetKey IconKey;
    public string Name;
    public string Describes;
}
public class UIOnEnableInfos : MonoBehaviour, IEventSystem
{
    EUIMapDGIDType DGID => EUIMapDGIDType.UIOnEnableInfos_Animation;
    CanvasGroup MainCanvasGroup => m_MainRoot.GetComponent<CanvasGroup>();
    [SerializeField]
    private RectTransform m_MainRoot = null;
    [SerializeField]
    private TextMeshProUGUI m_NameTxt = null;
    [SerializeField]
    private TextMeshProUGUI m_DescribesTxt = null;
    [SerializeField]
    private Image m_IconImg = null;

    private AssetKey m_CurIconKey = AssetKey.None;

    private void Awake()
    {
        GTools.EventSystemMgr.Subscribe(EEventSystemType.UIOnEnableInfos_Enter, this);
        GTools.EventSystemMgr.Subscribe(EEventSystemType.UIOnEnableInfos_Exit, this);
    }
    public void ReceptionEvent(EEventSystemType f_Event, object f_Params, string f_SendDesc)
    {
        var dataInfos = f_Params as UIOnEnableInfosShowData;
        switch (f_Event)
        {
            case EEventSystemType.UIOnEnableInfos_Enter:
                UpdateInfos(dataInfos);
                break;
            case EEventSystemType.UIOnEnableInfos_Exit:
                ExitClick();
                break;
            case EEventSystemType.EnumCount:
                break;
            default:
                break;
        }
    }

    public async void UpdateInfos(UIOnEnableInfosShowData f_Infos)
    {
        m_NameTxt.text = f_Infos.Name;
        m_DescribesTxt.text = f_Infos.Describes;

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_MainRoot);
        DOTween.Kill(DGID);

        DOTween.Kill(DGID);
        DOTween.To(() => 0.0f, value =>
        {
            MainCanvasGroup.alpha = value;

        }, 1.0f, 1.0f)
            .SetId(DGID)
            .OnStart(() =>
            {
                MainCanvasGroup.blocksRaycasts = true;
                MainCanvasGroup.interactable = true;
            });
        if (m_CurIconKey == f_Infos.IconKey)
        {
            return;
        }
        Release();

        if (GTools.TableMgr.TryGetAssetPath(f_Infos.IconKey, out var path) )
        {
            m_CurIconKey = f_Infos.IconKey;
            var sprite = await ILoadSpriteAsync.LoadAsync(path);
            if (m_CurIconKey != f_Infos.IconKey)
            {
                ILoadSpriteAsync.UnLoad(sprite);
                return;
            }
            m_IconImg.sprite = sprite;
        }
    }

    public void ExitClick()
    {
        m_CurIconKey = AssetKey.None;
        DOTween.Kill(DGID);
        DOTween.To(() => 0.0f, value =>
          {
              MainCanvasGroup.alpha = 1 - value;

          }, 1.0f, 1.0f)
            .SetId(DGID)
            .OnStart(() =>
            {
                MainCanvasGroup.interactable = false;
            })
            .OnComplete(() =>
            {
                MainCanvasGroup.blocksRaycasts = false;
                Release();
            });
    }
    public void Release()
    {
        if (m_IconImg.sprite != null)
        {
            ILoadSpriteAsync.UnLoad(m_IconImg.sprite);
            m_IconImg.sprite = null;
        }
    }
    private void Update()
    {
        if (m_CurIconKey == AssetKey.None)
        {
            return;
        }

        var pivotX = Input.mousePosition.x > Screen.width * 0.5f ? 1 : 0;
        var pivotY = Input.mousePosition.y > Screen.height * 0.5f ? 1 : 0;

        m_MainRoot.pivot = new Vector2(pivotX, pivotY);
        m_MainRoot.anchoredPosition = Input.mousePosition;
    }
}
