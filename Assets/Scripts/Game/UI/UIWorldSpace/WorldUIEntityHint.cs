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
        Entity = f_Target;
    }

    public override AssetKey AssetPrefabID => AssetKey.WorldUIEntityHint;

    public override EWorldObjectType ObjectType => EWorldObjectType.None;
    public override bool IsUpdateEnable => true;

    public int Key => Entity.Index;
    public float PersentBlood => CurBlood / (float)MaxBlood;
    public int MaxBlood => Entity.MaxBlood;
    public int CurBlood => Entity.CurrentBlood;
    public float CurPersentBlood = 0;
    public float CurPersentBloodTarget = 0;
    public string CurBloodHint = "";

    public float PersentMagic => CurMagic / (float)MaxMagic;
    public int MaxMagic => Entity.MaxMagic;
    public int CurMagic => Entity.CurrentMagic;
    public float CurPersentMagic = 0;
    public string CurMagicHint = "";
    public WorldObjectBaseData Entity = null;
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



        SetPosition(Entity.PointUp);
        SetForward(WorldPosition - GTools.MainCamera.transform.position);
    }
}
public class WorldUIEntityHint : ObjectPoolBase
{
    [SerializeField]
    private TextMeshPro m_TxtBloodHint = null;
    [SerializeField]
    private Image m_ImgBloodTarget = null;
    [SerializeField]
    private Image m_ImgCurBlood = null;

    [SerializeField]
    private TextMeshPro m_TxtMagicHint = null;
    [SerializeField]
    private Image m_ImgCurMagic = null;

    private WorldUIEntityHintData m_Data => GetData<WorldUIEntityHintData>();

    public void UpdateInfo()
    {
        m_TxtBloodHint.text = m_Data.CurBloodHint;
        m_ImgCurBlood.fillAmount = m_Data.PersentBlood;
        m_ImgBloodTarget.fillAmount = m_Data.CurPersentBloodTarget;


        m_TxtMagicHint.text = m_Data.CurMagicHint;
        m_ImgCurMagic.fillAmount = m_Data.PersentMagic;
    }
}
