using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class TerrainRenderData
{
    public TerrainRenderData(string f_Flag, int? f_Width = null, int? f_Height = null)
    {
        var size = WorldMapMgr.Ins.RowCol;
        m_Width = f_Width ?? size.y * 2;
        m_Height = f_Height ?? size.x;
        m_Flag = f_Flag;
    }
    private RenderTexture m_RT = null;
    private SpriteRenderer m_RawImage = null;
    private Texture2D m_Texture = null;
    private Sprite m_Sprite = null;
    private string m_Flag = "";
    private int m_Width = 0;
    private int m_Height = 0;
    public Transform Tran => m_RawImage.transform;
    public void Init()
    {
        // 生成图片部分
        Texture2D texture2D = new(m_Width, m_Height);
        texture2D.wrapMode = TextureWrapMode.Clamp;
        texture2D.filterMode = FilterMode.Point;

        for (int i = 0; i < m_Width; i++)
        {
            for (int j = 0; j < m_Height; j++)
            {
                Color color = new Color(0, 0, 0, 1);
                texture2D.SetPixel(i, j, color);
            }
        }
        // 对象部分
        var name = $"{m_Flag}";
        var rtObj = GameObject.Find(name) ?? new GameObject(m_Flag);

        if (!rtObj.TryGetComponent<SpriteRenderer>(out var rawImage))
        {
            rawImage = rtObj.AddComponent<SpriteRenderer>();
        }

        var sprite = Sprite.Create(texture2D, new(0, 0, m_Width, m_Height), new(0.5f, 0.5f));
        rawImage.sprite = sprite;

        texture2D.Apply();


        m_Texture = texture2D;
        m_RawImage = rawImage;
        m_Sprite = sprite;

        TerrainMgr.Ins.Mat.SetTexture(m_Flag, texture2D);
    }
    public RenderTexture GetRt()
    {
        if (m_RT == null)
        {
            m_RT = RenderTexture.GetTemporary(m_Width, m_Height);
            Graphics.Blit(m_Texture, m_RT);
        }

        return m_RT;
    }
    public void SaveSprite()
    {
        // 保存图片到本地
    }
    public void Destroy()
    {
        RenderTexture.ReleaseTemporary(m_RT);
        GameObject.Destroy(m_Sprite);
        GameObject.Destroy(m_Texture);
        GameObject.Destroy(m_RawImage.gameObject);
        m_RT = null;
        m_Texture = null;
        m_Sprite = null;
        m_RawImage = null;
        m_Flag = "";
        m_Width = 0;
        m_Height = 0;
    }
    public void SetPixel(int x, int y, Color col)
    {
        m_Texture.SetPixel(x, y, col);
        m_Texture.Apply();
        if (m_RT != null)
        {
            Graphics.Blit(m_Texture, m_RT);
        }
    }
    public void SetPosition(Vector3 f_WorldPos)
    {
        Tran.position = f_WorldPos;
    }
    public void SetScale(Vector3 f_Scale)
    {
        f_Scale.x /= 2;
        Tran.localScale = f_Scale;
    }
    public void SetRotation(Vector3 f_Angle)
    {
        Tran.localRotation = Quaternion.Euler(f_Angle);
    }
}
public class TerrainMgr : Singleton<TerrainMgr>
{
    Dictionary<int, TerrainRenderData> m_TerrainRenderData = new()
    {
        {
            0,
            new("_RoadTex")
        },
        //{
        //    1,
        //    new("_GrassTex")
        //}
    };
    public Material Mat = null;
    public Transform SpriteTran = null;
    public override void Awake()
    {
        base.Awake();
        Mat = Resources.Load<Material>("Shader/Custom_Terrain");
        var obj = new GameObject("_RoadTex");
        var sprite = obj.AddComponent<SpriteRenderer>();
        sprite.material = Mat;
        SpriteTran = obj.transform;

    }
    public override void Start()
    {
        base.Start();


        var chunkRowCol = WorldMapMgr.Ins.RowCol;

        var chunk0 = WorldMapMgr.Ins.GetChunkPoint(0);
        var chunkEnd = WorldMapMgr.Ins.GetChunkPoint(chunkRowCol.x * chunkRowCol.y - 1);

        var rtWidth = chunkRowCol.x;
        var rtHeight = chunkRowCol.y;

        var worldSize = chunkEnd - chunk0;
        var worldPos = (chunkEnd + chunk0) * 0.5f;



        var worldPos2 = worldPos + new Vector3(0, 0.1f, 0);
        var scale = new Vector2(worldSize.x, worldSize.z) * 100 / new Vector2(rtHeight, rtWidth);
        var localScale = new Vector3(scale.x, scale.y, 1);
        var rotation = new Vector3(90, 0, 0);

        foreach (var item in m_TerrainRenderData)
        {
            item.Value.Init();
            item.Value.SetPosition(worldPos2);
            item.Value.SetScale(localScale);
            item.Value.SetRotation(rotation);
        }

        SetColorTest();
    }


    public void SetChunkColor(int x, int y, Color color)
    {
        var resultY = y * 2 + x % 2;

        m_TerrainRenderData[0].SetPixel(resultY, x, color);
    }



    public void SetColorTest()
    {
        WorldMapMgr.Ins.LoopChunk((item) =>
        {
            if (item.Value.IsAlreadyType(EWorldObjectType.Road))
            {
                var rowCol = WorldMapMgr.Ins.GetRowCol(item.Value.Index);
                SetChunkColor(rowCol.x, rowCol.y, Color.gray);
            }
        });
    }
}
