using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using DG.Tweening;

public class UIGameHelp : UIWindow
{
    private EDGWorldID DGMoveImgID => EDGWorldID.UIGameHelp;
    [SerializeField]
    private int m_CurIndex = 0;
    [SerializeField]
    private GameHelpInfo m_GameInfo = null;
    [SerializeField]
    private RectTransform m_HelpInfoRoot = null;
    private HorizontalLayoutGroup ItemRootHor => m_HelpInfoRoot.GetComponent<HorizontalLayoutGroup>();
    [SerializeField]
    private RectTransform m_HelpInfoItem = null;
    [SerializeField]
    private Button m_NextBtn = null;
    [SerializeField]
    private Button m_LastBtn = null;
    [SerializeField]
    private Button m_ExitBtn = null;
    private List<RectTransform> m_ItemList = new();
    public override async UniTask AwakeAsync()
    {
        m_HelpInfoItem.gameObject.SetActive(false);
        GameManager.SetGameScale(0);
        m_NextBtn.onClick.RemoveAllListeners();
        m_LastBtn.onClick.RemoveAllListeners();
        m_ExitBtn.onClick.RemoveAllListeners();
        m_NextBtn.onClick.AddListener(NextHelpInfo);
        m_LastBtn.onClick.AddListener(LastHelpInfo);
        m_ExitBtn.onClick.AddListener(async ()=>
        {
            await GTools.UIWindowManager.UnloadWindowAsync(this);
        });
    }

    public override async UniTask OnShowAsync()
    {

    }

    public override async UniTask OnUnLoadAsync()
    {
        DOTween.Kill(DGMoveImgID);
        GameManager.SetGameScale(1);
        ClearGameHelpItem();
        await base.OnUnLoadAsync();

    }

    public async void InitHelpInfo(GameHelpInfo f_HelpInfo)
    {
        if (f_HelpInfo.Count == 0)
        {
            return;
        }
        m_CurIndex = 0;
        m_GameInfo = f_HelpInfo;
        ClearGameHelpItem();
        foreach (var item in f_HelpInfo.InfoAssetKeyList)
        {
            var obj = await CreateGameInfoItem(item);
            m_ItemList.Add(obj);
        }
        ApplyCurIndex(true);
        UpdateBtnStatus();
    }
    private void ClearGameHelpItem()
    {
        foreach (var item in m_ItemList)
        {
            DestroyGameInfoItem(item);
        }
        m_ItemList.Clear();
    }

    private void NextHelpInfo()
    {
        var toIndex = Mathf.Min(m_CurIndex + 1, m_GameInfo.Count - 1);
        if (toIndex == m_CurIndex)
        {
            return;
        }
        m_CurIndex = toIndex;
        UpdateBtnStatus();
        ApplyCurIndex();
    }
    private void LastHelpInfo()
    {
        var toIndex = Mathf.Max(m_CurIndex - 1, 0);
        if (toIndex == m_CurIndex)
        {
            return;
        }
        m_CurIndex = toIndex;
        UpdateBtnStatus();
        ApplyCurIndex();
    }
    private void UpdateBtnStatus()
    {
        m_NextBtn.interactable = m_CurIndex < m_GameInfo.Count - 1;
        m_LastBtn.interactable = m_CurIndex > 0;
    }
    private void ApplyCurIndex(bool f_IsForce = false)
    {
        DOTween.Kill(DGMoveImgID);
        var moveTime = f_IsForce ? 0 : 1.0f;
        var startPos = m_HelpInfoRoot.anchoredPosition.x;
        var toPos = (ItemRootHor.spacing + m_HelpInfoItem.sizeDelta.x) * m_CurIndex;
        //DOTween.To(() => 0.0f, slider =>
        //  {
        //      var pos = Mathf.Lerp(startPos, toPos, slider);
        //      m_HelpInfoRoot.anchoredPosition = Vector2.right * pos;

        //  }, 1.0f, moveTime)
        //    .SetId(DGMoveImgID);
        m_HelpInfoRoot.anchoredPosition = -Vector2.right * toPos;
    }
    private async UniTask<RectTransform> CreateGameInfoItem(EAssetKey f_ImgAssetPath)
    {
        var item = GameObject.Instantiate(m_HelpInfoItem, m_HelpInfoRoot);
        var sprite = await ILoadSpriteAsync.LoadAsync(f_ImgAssetPath);
        item.GetChildCom<Image>(EChildName.Img_Icon).sprite = sprite;
        item.gameObject.SetActive(true);
        return item;
    }
    private void DestroyGameInfoItem(RectTransform f_ObjTarget)
    {
        GameObject.Destroy(f_ObjTarget.gameObject);
    }
}
