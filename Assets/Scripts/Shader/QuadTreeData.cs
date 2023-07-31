using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "QuadTreeData", menuName = "UIMap/QuadTreeData")]
public class QuadTreeData : ScriptableObject
{
    public string DataName => $"{GetType()}";

    public int TreeNum => Mgr.TreeNum;
    public List<int> IndexList = new();
    public List<QuadTreeNode> NodeList = new();
    public QuadTreeNode RootNode => IndexList.Contains(0) ? NodeList[IndexList.IndexOf(0)] : AddTreeNode(0);
    public Dictionary<int, QuadTreeNode> TreeNodeList => GetData();

    public Dictionary<int, QuadTreeNode> GetData()
    {
        Dictionary<int, QuadTreeNode> dic = new();
        for (int i = 0; i < IndexList.Count; i++)
        {
            var key = IndexList[i];
            var value = NodeList[i];
            dic.Add(key, value);
        }
        return dic;
    }
    public void SaveData(Dictionary<int, QuadTreeNode> f_NodeList)
    {
        NodeList.Clear();
        IndexList.Clear();

        foreach (var item in f_NodeList)
        {
            NodeList.Add(item.Value);
            IndexList.Add(item.Key);
        }

    }



    QuadTreeMgr Mgr => QuadTreeMgr.Instance;
    #region 节点相关

    /// <summary>
    /// 添加一个节点
    /// </summary>
    /// <param name="f_ParentNode"></param>
    /// <param name="f_Point"></param>
    /// <param name="f_IsCover"></param>
    /// <returns></returns>
    public QuadTreeNode AddTreeNode(QuadTreeNode f_ParentNode, EQuadTreeNode f_Point, bool f_IsCover)
    {
        var parentNodeDepthIndex = Mgr.GetDepth(f_ParentNode.Index);

        var nodeTopDepthNum = Mgr.GetDepthTopNum(parentNodeDepthIndex + 1);

        var index = nodeTopDepthNum + TreeNum * parentNodeDepthIndex + (int)f_Point - 1;

        var node = new QuadTreeNode(index);

        AddDicNode(node, f_IsCover);

        return node;
    }

    /// <summary>
    /// 添加一个节点
    /// </summary>
    /// <param name="f_Index"></param>
    /// <returns></returns>
    public QuadTreeNode AddTreeNode(int f_Index)
    {
        if (f_Index > 0)
        {
            var parentIndex = Mgr.GetParentIndex(f_Index);
            var parentNode = Mgr.GetParentNode(f_Index);

            if (object.ReferenceEquals(parentNode, null))
            {
                AddTreeNode(parentIndex);
            }
        }

        var node = new QuadTreeNode(f_Index);

        if (!AddDicNode(node, false))
        {
            node = Mgr.GetNode(f_Index);
            Debug.Log($"添加节点失败，节点已经存在 index = {f_Index}");
        }


        return node;
    }

    /// <summary>
    /// 删除一个节点
    /// </summary>
    /// <param name="f_Node"></param>
    public void RemoveTreeNode(QuadTreeNode f_Node)
    {
        // 移除子节点
        var nodeList = f_Node.GetLeafNodes();
        foreach (var item in nodeList)
        {
            var tempItem = item;

            RemoveTreeNode(tempItem.Value);
            var node = Mgr.GetNode(tempItem.Value);
            RemoveDicNode(node);
        }
        f_Node.ClearLeafNodes();
        RemoveDicNode(f_Node);

        // 移除父节点引用
        var parent = Mgr.GetParentNode(f_Node.Index);
        var pos = Mgr.GetInParentPoint(f_Node.Index);
        parent.RemoveLeafNode(pos);
    }
    /// <summary>
    /// 删除一个节点
    /// </summary>
    /// <param name="f_Node"></param>
    public void RemoveTreeNode(int f_Index)
    {
        var node = Mgr.GetNode(f_Index);
        if (node != null)
        {
            RemoveTreeNode(node);
        }
        else
        {
            Debug.Log($"节点索引不存在 index = {f_Index}");
        }
    }
    /// <summary>
    /// 添加节点到缓存
    /// </summary>
    /// <param name="f_Node"></param>
    /// <param name="f_IsCover"></param>
    /// <returns></returns>
    public bool AddDicNode(QuadTreeNode f_Node, bool f_IsCover)
    {
        var index = f_Node.Index;
        var point = Mgr.GetInParentPoint(f_Node.Index);
        var parentNode = Mgr.GetParentNode(index);
        if (IndexList.Contains(index))
        {
            var pos = IndexList.IndexOf(index);
            var value = NodeList[pos];

            if (f_IsCover || object.ReferenceEquals(value, null))
            {
                NodeList[pos] = f_Node;
                parentNode.SetLeafNode(f_Node, point);
                f_Node.AddClick();
                return true;
            }
        }
        else
        {
            NodeList.Add(f_Node);
            IndexList.Add(index);
            parentNode?.SetLeafNode(f_Node, point);
            f_Node.AddClick();
            return true;
        }
        return false;
    }
    /// <summary>
    /// 从缓存中删除节点
    /// </summary>
    /// <param name="f_Node"></param>
    public void RemoveDicNode(QuadTreeNode f_Node)
    {
        var index = f_Node.Index;
        if (IndexList.Contains(index))
        {
            f_Node.DesClick();
            var pos = IndexList.IndexOf(index);
            IndexList.Remove(index);
            NodeList.RemoveAt(pos);
        }
        else
        {
            Debug.Log($"当前删除节点不存在 index = {index}");
        }
    }


#endregion



}
