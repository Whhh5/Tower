using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Monster_Boss1SkillItemData : UnityObjectData
{
    public class ItemData
    {
        public float Angle = 0.0f;
    }
    public Entity_Monster_Boss1SkillItemData() : base(0)
    {
    }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Monster_Boss1SkillItem;
    public override EWorldObjectType ObjectType => EWorldObjectType.Effect;
    private Entity_Monster_Boss1SkillItem EntityTarget => GetCom<Entity_Monster_Boss1SkillItem>();
    private Dictionary<int, ItemData> m_ItemAngleList = new();
    public int ItemCount = 0;
    public Vector3 RootPosition;
    public float RootRotation;
    public float ItemsAlpha = 0.0f;
    public void InitItemList(int f_ItemCount)
    {
        m_ItemAngleList = new();
        for (int i = 0; i < f_ItemCount; i++)
        {
            m_ItemAngleList.Add(i, new());
        }
        ItemCount = f_ItemCount;
        if (EntityTarget)
        {
            EntityTarget.InitItemList();
        }
    }
    public bool TryGetItemData(int f_Index, out ItemData f_ItemData)
    {
        return m_ItemAngleList.TryGetValue(f_Index, out f_ItemData);
    }
    public void SetItemRotation(int f_Index, float f_Angle)
    {
        if (!m_ItemAngleList.TryGetValue(f_Index, out var data))
        {
            return;
        }
        data.Angle = f_Angle;
        if (EntityTarget != null)
        {
            EntityTarget.SetItemRotation(f_Index);
        }
    }
    public void SetRootPosition(Vector3 f_Positon)
    {
        RootPosition = f_Positon;
        if (EntityTarget != null)
        {
            EntityTarget.SetRootPosition();
        }
    }
    public void SetRootRotation(float f_Rotation)
    {
        RootRotation = f_Rotation;
        if (EntityTarget != null)
        {
            EntityTarget.SetRootRotation();
        }
    }
    public void SetItemsAlpha(float f_Alpha)
    {
        ItemsAlpha = f_Alpha;
        if (EntityTarget != null)
        {
            EntityTarget.SetItemsAlpha();
        }
    }
    public float GetCurItemAlpha()
    {
        return ItemsAlpha;
    }
    public Vector3 GetWeaponRootPosition()
    {
        return RootPosition;
    }

    public float GetRootRotation()
    {
        return RootRotation;
    }
}
public class Entity_Monster_Boss1SkillItem : ObjectPoolBase
{
    private Entity_Monster_Boss1SkillItemData EntityData => GetData<Entity_Monster_Boss1SkillItemData>();
    [SerializeField]
    private Transform m_ChildItem;
    [SerializeField]
    private Transform m_ItemRoot;
    Dictionary<int, Transform> m_ChildItemList = new();
    private Color m_OriginalColor = Color.white;
    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        await base.OnStartAsync(f_Params);

        m_ChildItem.gameObject.SetActive(false);
        InitItemList();
        SetRootPosition();
        SetItemsAlpha();
        SetRootRotation();
    }

    public override async UniTask OnUnLoadAsync()
    {
        foreach (var item in m_ChildItemList)
        {
            GameObject.Destroy(item.Value.gameObject);
        }
        m_ChildItemList.Clear();

        await base.OnUnLoadAsync();
    }
    public void InitItemList()
    {
        if (m_ChildItemList.Count > 0)
        {
            return;
        }
        for (int i = 0; i < EntityData.ItemCount; i++)
        {
            var item = GameObject.Instantiate(m_ChildItem, m_ItemRoot);
            item.gameObject.SetActive(true);
            m_ChildItemList.Add(i, item);
            SetItemRotation(i);
        }
    }
    
    public void SetItemRotation(int f_Index)
    {
        if (!EntityData.TryGetItemData(f_Index, out var itemData))
        {
            return;
        }
        if (!m_ChildItemList.TryGetValue (f_Index, out var item))
        {
            return;
        }
        item.eulerAngles = Vector3.forward * itemData.Angle;
    }
    public void SetRootPosition()
    {
        m_ItemRoot.position = EntityData.RootPosition;
    }
    public void SetRootRotation()
    {
        m_ItemRoot.eulerAngles = Vector3.forward * EntityData.RootRotation;
    }
    public void SetItemsAlpha()
    {
        var color = m_OriginalColor;
        color.a = EntityData.ItemsAlpha;
        foreach (var item in m_ChildItemList)
        {
            var sprites = item.Value.GetComponentsInChildren<SpriteRenderer>();
            if (sprites == null || sprites.Length == 0)
            {
                continue;
            }
            foreach (var spriteCom in sprites)
            {
                spriteCom.color = color;
            }
        }
    }
}
