using B1;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class WorldDamageText : ObjectPoolBase
{
    [SerializeField]
    private TextMeshPro m_TextPro = null;
    [SerializeField]
    private Color m_Color = Color.white;
    [SerializeField]
    private float m_Time = 2;
    [SerializeField]
    private float m_Height = 2;
    [SerializeField]
    private AnimationCurve m_MoveCurveY = null;


    private float m_StartTime;
    private Vector3 m_StartPos;


    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
        m_StartTime = GTools.CurTime;
        m_StartPos = transform.position;
    }
    public override async UniTask OnStartAsync(params object[] f_Params)
    {
       
    }

    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
    }


    public override async void OnUpdate()
    {
        if (IsActively)
        {
            var scheme = (GTools.CurTime - m_StartTime) / m_Time;
            var curveValue = m_MoveCurveY.Evaluate(scheme);
            if (scheme > 1)
            {
                await GTools.AssetsMgr.UnLoadPrefabPoolAsync(this);
            }
            else
            {
                transform.position = m_StartPos + new Vector3(0, m_Height, 0) * curveValue;
                m_TextPro.color = new Color(m_Color.r, m_Color.g, m_Color.b, m_Color.a * (1 - curveValue));
                float scaleRange = Mathf.Clamp((1 - curveValue), 0.5f, 0.8f);
                transform.localScale = Vector3.one * scaleRange;
            }
            transform.forward = -GTools.MainCamera.transform.forward;
        }
    }

    public async UniTask SetParameters(string f_Content, EDamageType f_DamageType = EDamageType.None)
    {
        m_TextPro.text = f_Content;


        Color color = Color.gray;
        switch (f_DamageType)
        {
            case EDamageType.None:
                break;
            case EDamageType.Physical:
                color = Color.red;
                break;
            case EDamageType.Magic:
                color = Color.blue;
                break;
            case EDamageType.True:
                color = Color.white;
                break;
            case EDamageType.AddBlood:
                color = Color.green;
                break;
            default:
                break;
        }
        m_TextPro.color = color;
        m_Color = color;
    }


}
