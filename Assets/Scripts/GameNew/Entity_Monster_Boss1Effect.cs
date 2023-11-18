using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity_Monster_Boss1EffectData : UnityObjectData
{
    public class EffectData
    {
        public bool IsForward = true;
        public float Radius = 0.0f;
        public float Angle = 0.0f;
        public float Speed = 0.0f;
        public float Scale = 0.0f;
        public float Alpha = 0.0f;
        public float AlphaRatio = 0.0f;
        public float ThisAngle = 0.0f;
        public float ThisSpeed = 0.0f;
        public bool ThisForward = true;
        public Vector3 GetLocalPosition()
        {
            var angle = (IsForward ? 1 : -1) * Angle;
            var posX = Mathf.Cos(angle * Mathf.Deg2Rad * Speed);
            var posY = Mathf.Sin(angle * Mathf.Deg2Rad * Speed);
            return new Vector3(posX, posY, 0) * Radius;
        }
        public Vector3 GetLocalRotation()
        {
            var rot = ThisAngle * ThisSpeed * (ThisForward ? 1 : -1);
            return new Vector3(0, 0, rot);
        }
        public Vector3 GetLocalScale()
        {
            return Vector3.one * Scale;
        }
        public Color GetColor()
        {
            return new Color(1, 1, 1, Alpha * AlphaRatio);
        }
    }
    public Entity_Monster_Boss1EffectData() : base(0)
    {
    }
    public Entity_Monster_Boss1Data Initiator = null;

    public Vector2 SpeedSection => new(20.0f, 200.0f);
    public Vector2 RadiusSection => new(0.1f, 0.9f);
    public Vector2 ScaleSection => new(1.0f, 3.0f);
    public void InitData(float f_Radius)
    {
        AtkRadius = f_Radius;
        for (int i = 0; i < ChildCount; i++)
        {
            var initAngle = GTools.MathfMgr.GetRandomValue(0, 360);
            var initRadius = GTools.MathfMgr.GetRandomValue(f_Radius * RadiusSection.x, f_Radius * RadiusSection.y);
            var initLocalScale = GTools.MathfMgr.GetRandomValue(ScaleSection.x, ScaleSection.y);
            var initSpeed = GTools.MathfMgr.GetRandomValue(f_Radius * SpeedSection.x, f_Radius * SpeedSection.y);
            var alpha = initSpeed / (f_Radius * SpeedSection.y);
            initSpeed *= initRadius / f_Radius;
            initSpeed *= initLocalScale / ScaleSection.y;
            var dir = GTools.MathfMgr.GetRandomValue(0.0f, 1.0f) > 0.3f;
            m_ChildPositionList.Add(i, new()
            {
                Angle = initAngle,
                Radius = initRadius,
                IsForward = dir,
                Speed = initSpeed,
                Scale = initLocalScale,
                Alpha = alpha,
                ThisAngle = 0.0f,
                ThisForward = !dir,
                ThisSpeed = initSpeed * 2,
            });
        }
    }
    public Entity_Monster_Boss1Effect EntityTarget => GetCom<Entity_Monster_Boss1Effect>();
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Monster_Boss1Effect;
    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;

    public int ChildCount => 80;
    public float AtkRadius = 0.0f;
    private Dictionary<int, EffectData> m_ChildPositionList = new();
    private float m_CurAlpha = 0.0f;
    private string DGID => $"{EDGWorldID.Boss1EffectAlpha}_{LoadKey}";
    public override void OnLoad()
    {
        base.OnLoad();
        m_CurAlpha = 0.0f;
    }
    public override void AfterLoad()
    {
        base.AfterLoad();
        UpdateChildLocalPos();

        DOTween.Kill(DGID);
        var curAlpha = m_CurAlpha;
        var endAlpha = 1.0f;
        DOTween.To(() => 0.0f, slider =>
        {
            var alpha = Mathf.Lerp(curAlpha, endAlpha, slider);
            m_CurAlpha = alpha;
            SetColorAlpha(alpha);

        }, endAlpha, endAlpha - curAlpha)
            .SetId(DGID);
    }
    public override void UnLoad()
    {
        m_ChildPositionList.Clear();
        base.UnLoad();
    }
    public void Destroy()
    {
        DOTween.Kill(DGID);
        var curAlpha = m_CurAlpha;
        var endAlpha = 0.0f;
        DOTween.To(() => 0.0f, slider =>
        {
            var alpha = Mathf.Lerp(curAlpha, endAlpha, slider);
            m_CurAlpha = alpha;
            SetColorAlpha(alpha);

        }, 1.0f, curAlpha)
            .SetId(DGID)
            .OnComplete(()=>
            {
                ILoadPrefabAsync.UnLoad(this);
            });
    }

    public override bool IsUpdateEnable => true;
    public float CurAngle = 0.0f;
    public override void OnUpdate()
    {
        base.OnUpdate();

        foreach (var item in m_ChildPositionList)
        {
            item.Value.Angle += Time.deltaTime;
            item.Value.ThisAngle += Time.deltaTime;
        }
        UpdateChildLocalPos();




    }
    public void SetColorAlpha(float f_Alpha)
    {
        foreach (var item in m_ChildPositionList)
        {
            item.Value.AlphaRatio = f_Alpha;
        }
    }
    public void UpdateChildLocalPos()
    {
        if (EntityTarget != null)
        {
            EntityTarget.UpdateChildLocalPos();
        }
    }
    public void SetChildAngle(int f_Index, float f_Angle)
    {
        m_ChildPositionList[f_Index].Angle = f_Angle;
    }
    public bool TryGetChildPosData(int f_Index, out EffectData f_Result)
    {
        return m_ChildPositionList.TryGetValue(f_Index, out f_Result);
    }
}

public class Entity_Monster_Boss1Effect : ObjectPoolBase
{
    private Entity_Monster_Boss1EffectData EntityData => GetData<Entity_Monster_Boss1EffectData>();
    [SerializeField]
    private Transform m_MainRange = null;
    [SerializeField]
    private List<Transform> m_Items = new();
    [SerializeField]
    private List<Transform> m_InsList = new();


    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        await base.OnStartAsync(f_Params);
        m_MainRange.localScale = EntityData.AtkRadius * Vector3.one;
        foreach (var item in m_Items)
        {
            item.gameObject.SetActive(false);
        }
        for (int i = 0; i < EntityData.ChildCount; i++)
        {
            var index = GTools.MathfMgr.GetRandomValue(0, m_Items.Count);
            var item = m_Items[index];
            var insItem = GameObject.Instantiate(item, item.parent);
            insItem.gameObject.SetActive(true);
            m_InsList.Add(insItem);
        }

    }
    public override async UniTask OnUnLoadAsync()
    {
        foreach (var item in m_InsList)
        {
            GameObject.Destroy(item.gameObject);
        }
        m_InsList.Clear();
        await base.OnUnLoadAsync();
    }
    public void UpdateChildLocalPos()
    {
        for (int i = 0; i < EntityData.ChildCount; i++)
        {
            if (!EntityData.TryGetChildPosData(i, out var data))
            {
                continue;
            }
            var item = m_InsList[i];
            item.localPosition = data.GetLocalPosition();
            item.eulerAngles = data.GetLocalRotation();
            item.localScale = data.GetLocalScale();
            item.GetComponent<SpriteRenderer>().color = data.GetColor();
        }
    }
}
