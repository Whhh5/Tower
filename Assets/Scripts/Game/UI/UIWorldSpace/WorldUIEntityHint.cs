using System.Collections;
using System.Collections.Generic;
using B1.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIEntityHintData : UnityObjectData
{
    public WorldUIEntityHintData(int f_Index, WorldObjectBaseData f_Target) : base(f_Index)
    {
        TargetEntity = f_Target;
        Color color = Color.white;
        switch (f_Target.LayerMask)
        {
            case ELayer.Default:
                break;
            case ELayer.TransparentFX:
                break;
            case ELayer.IgnoreRatCast:
                break;
            case ELayer.___1:
                break;
            case ELayer.Water:
                break;
            case ELayer.UI:
                break;
            case ELayer.Terrain:
                break;
            case ELayer.Enemy:
                color = Color.red;
                break;
            case ELayer.Player:
                color = Color.green;
                break;
            case ELayer.FlyingProp:
                break;
            case ELayer.Tower:
                break;
            case ELayer.All:
                break;
            default:
                break;
        }
        UpdateBloodColor(color);
    }

    public override EAssetKey AssetPrefabID => EAssetKey.WorldUIEntityHint;

    public override EWorldObjectType ObjectType => EWorldObjectType.None;
    public override bool IsUpdateEnable => true;

    public int Key => TargetEntity.Index;
    public float PersentBlood => CurBlood / (float)MaxBlood;
    public int MaxBlood => TargetEntity.MaxBlood;
    public int CurBlood => TargetEntity.CurrentBlood;
    public float CurPersentBlood = 0;
    public float CurPersentBloodTarget = 0;
    public string CurBloodHint = "";

    public float PersentMagic => CurMagic / (float)MaxMagic;
    public int MaxMagic => TargetEntity.MaxMagic;
    public int CurMagic => TargetEntity.CurrentMagic;
    public float CurPersentMagic = 0;
    public string CurMagicHint = "";
    public WorldObjectBaseData TargetEntity = null;
    public WorldUIEntityHint Target => GetCom<WorldUIEntityHint>();

    public void UpdateInfo()
    {
        if (PersentBlood > CurPersentBlood)
        {
            CurPersentBloodTarget = PersentBlood;
        }
        else
        {
            CurPersentBlood = PersentBlood;
        }
        CurBloodHint = $"{CurBlood}/{MaxBlood}";
        CurMagicHint = $"{CurMagic}/{MaxMagic}";
    }

    public override void AfterLoad()
    {
        base.AfterLoad();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        { // UpdateInfo()

            CurPersentBlood = Mathf.MoveTowards(CurPersentBlood, PersentBlood, Time.deltaTime * 0.2f);
            CurPersentBloodTarget = Mathf.MoveTowards(CurPersentBloodTarget, PersentBlood, Time.deltaTime * 0.2f);


            if (Target != null)
            {
                Target.UpdateInfo();
            }
        }



        var uguiPos = IUIUtil.GetUGUIPosByWorld(TargetEntity.PointUp, true);
        var worldPos = IUIUtil.GetWorldPosBuyUGUIPos(uguiPos);
        SetPosition(worldPos);
    }
    public Color MainBloodColor = Color.white;
    private void UpdateBloodColor(Color f_ToColor)
    {
        MainBloodColor = f_ToColor;
        if (Target != null)
        {
            Target.UpdateBloodColor();
        }
    }
}
public class WorldUIEntityHint : ObjectPoolBase
{
    [SerializeField]
    private TextMeshProUGUI m_TxtBloodHint = null;
    [SerializeField]
    private Image m_ImgBloodTarget = null;
    [SerializeField]
    private Image m_ImgCurBlood = null;
    [SerializeField]
    private Image m_ImgBloodMiddleTarget = null;
    [SerializeField]
    private Image m_ImgCurUpBlood = null;

    [SerializeField]
    private TextMeshProUGUI m_TxtMagicHint = null;
    [SerializeField]
    private Image m_ImgCurMagic = null;

    private WorldUIEntityHintData EntityData => GetData<WorldUIEntityHintData>();

    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        await base.OnStartAsync(f_Params);
        UpdateBloodColor();

    }
    public void UpdateInfo()
    {
        m_TxtBloodHint.text = EntityData.CurBloodHint;
        m_ImgCurBlood.fillAmount = EntityData.PersentBlood;
        m_ImgBloodTarget.fillAmount = EntityData.CurPersentBloodTarget;


        m_TxtMagicHint.text = EntityData.CurMagicHint;
        m_ImgCurMagic.fillAmount = EntityData.PersentMagic;
    }

    public void UpdateBloodColor()
    {
        var color1 = m_ImgBloodMiddleTarget.color.a;
        var tocolor1 = EntityData.MainBloodColor;
        tocolor1.a = color1;
        m_ImgBloodMiddleTarget.color = tocolor1;

        color1 = m_ImgCurUpBlood.color.a;
        tocolor1.a = color1;
        m_ImgCurUpBlood.color = tocolor1;
    }
}
