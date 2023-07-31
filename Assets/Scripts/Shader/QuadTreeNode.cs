using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class QuadTreeNode
{
    [SerializeField]
    private int TopLeftIndex = -1;
    [SerializeField]
    private int TopRightIndex = -1;
    [SerializeField]
    private int BottomLeftIndex = -1;
    [SerializeField]
    private int BottomRightIndex = -1;

    // 索引
    [SerializeField]
    public int Index;

    [SerializeField]
    public int AreaID;


    /// <summary>
    /// 当前节点是否被开启
    /// </summary>
    public bool IsActive { get; private set; }
    /// <summary>
    /// 是否显示特效
    /// </summary>
    public bool IsEffect { get; private set; }



    public Vector3 Position;
    public float Radius = 0.2f;
    public float AreaBlend = 0.05f;




    public QuadTreeNode(int f_Index)
    {
        Index = f_Index;
    }

    public void InitParams()
    {
        IsActive = IsEffect = false;
    }


    public void SetLeafNode(QuadTreeNode f_Target, EQuadTreeNode f_Point)
    {
        switch (f_Point)
        {
            case EQuadTreeNode.None:
                break;
            case EQuadTreeNode.TopLeft:
                TopLeftIndex = f_Target.Index;
                break;
            case EQuadTreeNode.TopRight:
                TopRightIndex = f_Target.Index;
                break;
            case EQuadTreeNode.BottomLeft:
                BottomLeftIndex = f_Target.Index;
                break;
            case EQuadTreeNode.BottomRight:
                BottomRightIndex = f_Target.Index;
                break;
            default:
                break;
        }
    }
    public int RemoveLeafNode(EQuadTreeNode f_Point)
    {
        int node = -1;
        switch (f_Point)
        {
            case EQuadTreeNode.None:
                break;
            case EQuadTreeNode.TopLeft:
                node = TopLeftIndex;
                TopLeftIndex = -1;
                break;
            case EQuadTreeNode.TopRight:
                node = TopRightIndex;
                TopRightIndex = -1;
                break;
            case EQuadTreeNode.BottomLeft:
                node = BottomLeftIndex;
                BottomLeftIndex = -1;
                break;
            case EQuadTreeNode.BottomRight:
                node = BottomRightIndex;
                BottomRightIndex = -1;
                break;
            default:
                break;
        }
        return node;
    }
    public List<int> ClearLeafNodes()
    {
        List<int> nodeList = new();
        for (int i = 0; i < (int)EQuadTreeNode.EnumCount; i++)
        {
            var removeNode = RemoveLeafNode((EQuadTreeNode)i);
            if (!object.Equals(removeNode, -1))
            {
                nodeList.Add(removeNode);
            }
        }

        return nodeList;
    }
    public Dictionary<EQuadTreeNode, int> GetLeafNodes()
    {
        Dictionary<EQuadTreeNode, int> nodeList = new();


        if (!object.Equals(TopLeftIndex, -1))
        {
            nodeList.Add(EQuadTreeNode.TopLeft, TopLeftIndex);
        }
        if (!object.Equals(TopRightIndex, -1))
        {
            nodeList.Add(EQuadTreeNode.TopRight, TopRightIndex);
        }
        if (!object.Equals(BottomLeftIndex, -1))
        {
            nodeList.Add(EQuadTreeNode.BottomLeft, BottomLeftIndex);
        }
        if (!object.Equals(BottomRightIndex, -1))
        {
            nodeList.Add(EQuadTreeNode.BottomRight, BottomRightIndex);
        }

        return nodeList;
    }

    /// <summary>
    /// 当前节点被添加事件
    /// </summary>
    public void AddClick()
    {
        Debug.Log($"当前节点添加 index = {Index}");
    }
    /// <summary>
    /// 移除节点事件
    /// </summary>
    public void DesClick()
    {
        Debug.Log($"当前节点删除 index = {Index}");
    }
    public void SetCurStatus(bool f_State)
    {
        if (IsActive != f_State)
        {
            IsActive = f_State;
            updateCurSatus();
        }
    }
    public void updateCurSatus()
    {

    }

    public void ChangeEffectStatus(bool f_State)
    {
        IsEffect = f_State;
    }
}
