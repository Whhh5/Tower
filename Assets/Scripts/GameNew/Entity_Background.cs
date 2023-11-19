using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_BackgroundData : UnityObjectData
{
    public Entity_BackgroundData() : base(0)
    {
    }
    public override EAssetKey AssetPrefabID => EAssetKey.Entity_Background;
    public override EWorldObjectType ObjectType => EWorldObjectType.None;


}
public class Entity_Background : ObjectPoolBase
{
    private class ElementData
    {
        public Transform Item = null;
        public float Speed;
        public float Rotation;
    }
    [SerializeField]
    private List<Transform> m_ElementItemList = new();

    [SerializeField]
    private List<ElementData> m_CurElementList = new();



    public override void OnUpdate()
    {
        base.OnUpdate();


    }

    private void CreateElement()
    {
        var index = GTools.MathfMgr.GetRandomValue(0, m_ElementItemList.Count - 1);
        var item = m_ElementItemList[index];
        var eleData = new ElementData
        {
            Item = item
        };




        m_CurElementList.Add(eleData);
    }
}
