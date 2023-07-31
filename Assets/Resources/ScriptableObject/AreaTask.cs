using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System.Linq;

public class AreaTask : SerializedMonoBehaviour
{
    public Dictionary<Texture2D, Vector2> m_SpriteList = new();

    public Vector2Int m_MapSize = Vector2Int.zero;


    public RawImage m_MainTex = null;


    public Material m_FrameMat = null;


    public RectTransform m_Target = null;

    RenderTexture m_CurRt = null;

    [Button]
    public void Play()
    {

        var mainScale = 1;
        

        foreach (var item in m_SpriteList)
        {
            Shader shader = Shader.Find("Custom/AddColor");
            Material mat = new Material(shader);
            mat.SetFloat("_MainScale", mainScale);

            var rt2 = RenderTexture.GetTemporary((int)(m_MapSize.x / mainScale), (int)(m_MapSize.y / mainScale));
            var startPos = item.Value;

            mat.SetTexture("_SourceTex", m_CurRt);
            mat.SetTexture("_AreaTex", item.Key);
            mat.SetVector("_StartPoint", startPos);

            RenderTexture.ReleaseTemporary(m_CurRt);

            Graphics.Blit(null, rt2, mat);
            m_CurRt = rt2;
            m_FrameMat.SetTexture("_MainTex", m_CurRt);
            m_Target.gameObject.SetActive(false);
            m_Target.gameObject.SetActive(true);

            GameObject.DestroyImmediate(mat);
        }


        m_MainTex.texture = m_CurRt;




    }




}
