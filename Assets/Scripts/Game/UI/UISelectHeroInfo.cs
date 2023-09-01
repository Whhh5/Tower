using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UISelectHeroInfo : MonoBehaviour
{
    private WorldObjectBaseData t_targetData = null;
    // Start is called before the first frame update
    [SerializeField]
    private Image Img_Icon = null;
    [SerializeField]
    public TextMeshProUGUI name = null;
    [SerializeField]
    public TextMeshProUGUI attack = null;
    [SerializeField]
    public TextMeshProUGUI blood = null;
    [SerializeField]
    public Slider bloodSlider = null;
    [SerializeField]
    public TextMeshProUGUI magic = null;
    [SerializeField]
    public Slider magicSlider = null;
    [SerializeField]
    public TextMeshProUGUI speed = null;

    [SerializeField]
    private RectTransform layoutBuffRect = null;
    [SerializeField]
    private GameObject ItemBuff = null;

    [SerializeField]
    private GameObject BackGround = null;

    private List<GameObject> buffObjectList = new List<GameObject>();

    public static UISelectHeroInfo Ins = null;

    private Dictionary<AssetKey, SpriteAsset<Sprite>> sprite_dir = null;
    void Start()
    {
        Ins = this;
    }

    public void SetData(WorldObjectBaseData data)
    {
        if (Ins != null)
        {
            Ins.t_targetData = data;
            ShowAnim();
        }
    }
    public void UpdateHeroInfo()
    {
        if (t_targetData == null)
        {
            return;
        }

        UpdatePropInfo();
        UpdateBuffInfo();
    }

    public void ShowAnim()
    {
        BackGround.SetActive(t_targetData != null);
        BackGround.transform.localScale = Vector3.one;
        DOTween.To(() => 0.1f, (value) =>
        {
            BackGround.transform.localScale = Vector3.Lerp(0.8f * Vector3.one, Vector3.one, value);

        }, 1f, 0.2f);
    }

    private void UpdatePropInfo()
    {
        blood.text = $"{t_targetData.CurrentBlood}/{t_targetData.MaxBlood}";
        bloodSlider.value = (float)t_targetData.CurrentBlood/ (float)t_targetData.MaxBlood;
        attack.text= $"{t_targetData.CurHarm}";
        magic.text = $"{t_targetData.CurrentMagic}/{ t_targetData.MaxMagic}";
        magicSlider.value = (float)t_targetData.CurrentMagic / (float)t_targetData.MaxMagic;
        speed.text = $"{t_targetData.CurAnimaSpeed}";
    }

    private void UpdateBuffInfo()
    {
        Dictionary<EBuffType, Effect_BuffBaseData> buffs = t_targetData.GetBuff();
        Dictionary<EBuffType, Effect_BuffBaseData>.KeyCollection buffKeys = buffs.Keys;
        int buffCount = buffKeys.Count;
        int counter = 0;
        foreach (EBuffType e in buffKeys){
            counter += 1;
            if(counter> buffObjectList.Count)
            {
                if (ItemBuff == null)
                {
                    return;
                }
                GameObject buffObject = GameObject.Instantiate(ItemBuff, layoutBuffRect);
                buffObjectList.Add(buffObject);
            }
            ShowBuff(e, counter-1);
        }
        for(int i = counter;i< buffObjectList.Count;i++)
        {
            buffObjectList[i].SetActive(false);
        }
    }
    private void ShowBuff(EBuffType eBuff,int index)
    {
        if(index <= buffObjectList.Count-1)
        {
            GameObject buffObect = buffObjectList[index];
            buffObect.SetActive(true);
            Transform iconTransform = buffObect.transform.Find("Img_Icon");
            Image icon = iconTransform.GetComponent<Image>();
            
            if (TableMgr.Ins.TryGetBuffInfo(eBuff,out var info)){
               Sprite sp = GetSprite(info.IconPath);
               icon.sprite = sp;
            }
        }
    }

    private Sprite GetSprite(AssetKey assetKey)
    {
        if (sprite_dir == null)
        {
            sprite_dir = new Dictionary<AssetKey, SpriteAsset<Sprite>>();
        }
        if(sprite_dir.TryGetValue(assetKey,out SpriteAsset<Sprite> spriteAsset))
        {
            spriteAsset.refrence += 1;
            return spriteAsset.asset;
        }
        else
        {
            if (TableMgr.Ins.GetAssetPath(assetKey,out var path))
            {
                Sprite sp = Resources.Load<Sprite>(path);
                sprite_dir.Add(assetKey, new SpriteAsset<Sprite>(1, sp, assetKey));
                return sp;
            }
            else
            {
                return null;
            }
        }
    }
    private void Update()
    {
        UpdateHeroInfo();
    }

}

public class SpriteAsset<T> {
    public int refrence = 0;
    public T asset;
    public AssetKey key;
    public SpriteAsset(int refrence , T asset, AssetKey  key){
        this.refrence = refrence;
        this.asset = asset;
        this.key = key;
    }
}

