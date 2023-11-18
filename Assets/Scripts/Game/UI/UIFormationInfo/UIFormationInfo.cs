using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1.UI;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;

public class UIFormationData
{
    private RectTransform item = null;
    private TextMeshProUGUI m_Title => item.GetChildCom<TextMeshProUGUI>(EChildName.Txt_Title);
    private TextMeshProUGUI m_Describes => item.GetChildCom<TextMeshProUGUI>(EChildName.Txt_Describes);
    private RectTransform m_Point => item.GetChildCom<RectTransform>(EChildName.Tran_Point);
    private RectTransform m_PointItem => m_Point.GetChildCom<RectTransform>(EChildName.Item_Point);
    private List<RectTransform> m_PointItemList = new();
    private Vector2 m_ItemSize = Vector2.zero;
    private Vector2 m_ItemInterval = Vector2.one * 10;
    public void InitData(EFormationType f_FormationType, RectTransform f_Item)
    {
        if (!GTools.FormationMgr.TryGetFormationInfo(f_FormationType, out var formationInfo))
        {
            return;
        }
        item = GameObject.Instantiate(f_Item, f_Item.parent);
        m_ItemSize = m_PointItem.sizeDelta;
        m_PointItem.gameObject.SetActive(false);
        InitFormation(formationInfo.ArrayIndex, formationInfo.CentreIndex);
        m_Title.text = formationInfo.Name;
        m_Describes.text = formationInfo.Describes;
        item.gameObject.SetActive(true);
    }
    public void Destroy()
    {
        foreach (var item in m_PointItemList)
        {
            var image = item.GetChildCom<Image>(EChildName.Img_Icon);
            ILoadSpriteAsync.UnLoad(image.sprite);
            GameObject.Destroy(item.gameObject);
        }
        m_PointItemList.Clear();
        GameObject.Destroy(item.gameObject);
    }
    private void InitFormation(int[,] f_TargetArray, int f_CentrePoint)
    {
        var size = m_ItemSize + m_ItemInterval;
        var row = f_TargetArray.GetLength(0);
        var col = f_TargetArray.GetLength(1);
        var startPos = new Vector2
            (
                -(col - 1 + 0.5f) * size.x,
                (row - 1) * size.y
            );
        var centreRow = f_CentrePoint / col;
        var maxWhH = size * new Vector2(row - 1, col - 1);
        for (int i = 0; i < row; i++)
        {
            var posY = startPos.y - i * size.y - maxWhH.y * 0.5f;
            var posXOffset = centreRow % 2 == i % 2 ? (0.5f * size.y) : 0;
            for (int j = 0; j < col; j++)
            {
                var posX = startPos.x + j * size.x + posXOffset + maxWhH.x * 0.5f;
                var pos = new Vector2(posX, posY);
                var value = f_TargetArray[i, j];
                if (GTools.TableMgr.TryGetHeroVocationalInfo((EHeroVocationalType)value, out var vocationalInfo))
                {
                    InitPointItem(pos, vocationalInfo.IconID);
                }
                else
                {
                    InitPointItem(pos, null);
                }
            }
        }
    }
    private async void InitPointItem(Vector2 f_LocalPos, EAssetKey? f_IconKey)
    {
        var pointItem = GameObject.Instantiate(m_PointItem, m_PointItem.parent);
        pointItem.anchoredPosition = f_LocalPos;
        var image = pointItem.GetChildCom<Image>(EChildName.Img_Icon);
        pointItem.gameObject.SetActive(true);
        m_PointItemList.Add(pointItem);
        var sprite = await ILoadSpriteAsync.LoadAsync(f_IconKey ?? EAssetKey.None);
        image.enabled = sprite != null;
        image.sprite = sprite;

    }
}
public class UIFormationInfo : UIWindow
{
    [SerializeField]
    private RectTransform m_FormationItem = null;
    [SerializeField]
    private Button m_CloseBtn = null;
    private Dictionary<EFormationType, UIFormationData> m_FormationList = new();
    public override async UniTask AwakeAsync()
    {
        GameManager.SetGameScale(0);
        m_FormationItem.gameObject.SetActive(false);
        for (int i = 0; i < (int)EFormationType.EnumCount; i++)
        {
            var formationType = (EFormationType)i;
            var item = new UIFormationData();
            item.InitData(formationType, m_FormationItem);
            m_FormationList.Add(formationType, item);
        }
        m_CloseBtn.onClick.RemoveAllListeners();
        m_CloseBtn.onClick.AddListener(async () =>
        {
            await UIWindowManager.Ins.UnloadWindowAsync(this);
        });
    }

    public override async UniTask OnShowAsync()
    {

    }

    public override async UniTask OnUnLoadAsync()
    {
        foreach (var item in m_FormationList)
        {
            item.Value.Destroy();
        }
        m_FormationList.Clear();
        await base.OnUnLoadAsync();
        GameManager.SetGameScale(1);
    }
}
