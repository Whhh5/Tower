using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EQuadTreeNode
{
    None,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    EnumCount,
}
public class QuadTreeMgr
{
    public string ResPath = $"ScriptableObject/";
    public string AssetPath => $"Assets/Resources/{ResPath}";
    public static QuadTreeMgr Instance = new();


    public void StartExecute(string f_AssetName)
    {
        AssetData = Resources.Load<QuadTreeData>($"{ResPath}{f_AssetName}");

        if (AssetData == null)
        {
            return;
        }


        foreach (var item in TreeNodeList)
        {
            item.Value.InitParams();
        }
        Mat_Range ??= Resources.Load<Material>($"{ResPath}Custom_AreaShader");
        EdgeMat ??= Resources.Load<Material>($"{ResPath}Custom_EdgeShader");
        Mat_Edge ??= Resources.Load<Material>($"{ResPath}Custom_EdgeFrameShader");
        Mat_Frame ??= Resources.Load<Material>($"{ResPath}Custom_FrameShader");
        Mat_EnhanceEdge ??= Resources.Load<Material>($"{ResPath}Custom_EnhanceEdgeShader");
        Mat_Blurred ??= Resources.Load<Material>($"{ResPath}Custom_BlurredShader");

    }
    public void StopExecute()
    {
        if (AssetData != null)
        {
            Resources.UnloadAsset(AssetData);
            AssetData = null;
        }
    }


    public QuadTreeData AssetData = null;
    public int TreeNum = 4;

    /// <summary>
    /// 节点缓存
    /// </summary>
    public Dictionary<int, QuadTreeNode> TreeNodeList => AssetData?.GetData();
    /// <summary>
    /// 根节点
    /// </summary>
    public QuadTreeNode RootNode => AssetData?.RootNode;


    /// <summary>
    /// 更新所有节点状态
    /// </summary>
    public void UpdateAllNodeStatus()
    {
        var allLeafNodes = GetAllLeafNodes();

        foreach (var item in allLeafNodes)
        {
            var tempItem = item;
            UpdateNodeStatus(tempItem);
        }


    }


    #region 节点相关


    /// <summary>
    /// 获取叶节点
    /// </summary>
    /// <param name="f_Index"></param>
    /// <returns></returns>
    public int[] GetLeafNodeIndex(int f_Index)
    {
        var depth = GetDepth(f_Index);

        var depthIndex = GetDepthIndex(depth, f_Index);

        var depthNum = GetDepthTopNum(depth + 1);

        var startIndex = depthNum + depthIndex * TreeNum;

        int[] nodes = new int[TreeNum];
        for (int i = 0; i < TreeNum; i++)
        {
            nodes[i] = startIndex + i;
        }

        return nodes;
    }
    /// <summary>
    /// 获取当前节点在父节点的哪个位置
    /// </summary>
    /// <param name="f_Index"></param>
    /// <returns></returns>
    public EQuadTreeNode GetInParentPoint(int f_Index)
    {
        return (EQuadTreeNode)(f_Index % TreeNum + 1);
    }
    /// <summary>
    /// 获取深度
    /// </summary>
    /// <returns></returns>
    public int GetDepth(int f_Index)
    {
        int depth = Mathf.CeilToInt((float)f_Index / TreeNum);
        if (f_Index > TreeNum)
        {
            var t = Mathf.Log(TreeNum, f_Index);
            depth = Mathf.CeilToInt(Mathf.Log(f_Index, TreeNum));
        }
        return Mathf.CeilToInt(depth);
    }

    /// <summary>
    /// 获取当前深度上的索引
    /// </summary>
    /// <returns></returns>
    public int GetDepthIndex(int f_Depth, int f_Index)
    {
        var topNum = GetDepthTopNum(f_Depth);

        return f_Index - topNum;
    }

    /// <summary>
    /// 获取当前深度以上有多少节点
    /// </summary>
    /// <param name="f_Depth"></param>
    /// <returns></returns>
    public int GetDepthTopNum(int f_Depth)
    {
        var num = 0;
        for (int i = 0; i < f_Depth; i++)
        {
            num += (int)Mathf.Pow(4, i);
        }
        return num;
    }

    /// <summary>
    /// 获取当前深度节点最大数量
    /// </summary>
    /// <param name="f_Node"></param>
    /// <returns></returns>
    public int GetDepthMaxNum(int f_Depth)
    {
        int index = TreeNum * Mathf.Max(f_Depth - 1, 0);
        return index;
    }

    /// <summary>
    /// 获取父节点
    /// </summary>
    /// <param name="f_ChildNode"></param>
    /// <returns></returns>
    public QuadTreeNode GetParentNode(int f_Index) // 33
    {
        var parentIndex = GetParentIndex(f_Index);

        if (TreeNodeList.TryGetValue(parentIndex, out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 获取父节点的索引
    /// </summary>
    /// <param name="f_Index"></param>
    /// <returns></returns>
    public int GetParentIndex(int f_Index) // 33
    {
        var depth = GetDepth(f_Index); // 2

        var depthIndex = GetDepthIndex(depth, f_Index);

        var depthTopNum = GetDepthTopNum(depth);

        var parentIndex = depthTopNum - 1 + depthIndex / TreeNum;

        return parentIndex;
    }


    /// <summary>
    /// 通过索引获得节点
    /// </summary>
    /// <param name="f_Index"></param>
    /// <returns></returns>
    public QuadTreeNode GetNode(int f_Index)
    {
        if (TreeNodeList.TryGetValue(f_Index, out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }


    /// <summary>
    /// 获取当前最大的索引值
    /// </summary>
    /// <returns></returns>
    public int GetMaxIndex()
    {
        var maxIndex = 0;

        foreach (var item in TreeNodeList)
        {
            maxIndex = Mathf.Max(item.Key, maxIndex);
        }

        return maxIndex;
    }



    /// <summary>
    /// 获取所有兄弟节点
    /// </summary>
    /// <param name="f_Node"></param>
    /// <returns></returns>
    public Dictionary<EQuadTreeNode, int> GetBrotherNodes(QuadTreeNode f_Node)
    {
        var parent = GetParentNode(f_Node.Index);
        var nodes = parent?.GetLeafNodes() ?? new();

        return nodes;
    }

    /// <summary>
    /// 获取最大深度的所有节点索引
    /// </summary>
    public List<QuadTreeNode> GetAllLeafNodes()
    {
        var maxIndex = GetMaxIndex();

        var maxDepth = GetDepth(maxIndex);

        var depthNums = GetDepthMaxNum(maxDepth);

        var depthTopNum = GetDepthTopNum(depthNums);

        List<QuadTreeNode> nodeIndex = new();

        for (int i = depthTopNum; i < maxIndex + 1; i++)
        {
            if (TreeNodeList.TryGetValue(i, out var value))
            {
                nodeIndex.Add(value);
            }
        }

        return nodeIndex;
    }


    /// <summary>
    /// 深度遍历，拿到串上，第一个未解锁的节点
    /// </summary>
    public Dictionary<int, QuadTreeNode> GetDepthLoopFirstActive(QuadTreeNode f_Node)
    {
        Dictionary<int, QuadTreeNode> nodes = new();

        var curNodeActive = f_Node.IsActive;

        if (curNodeActive)
        {
            var leafNodes = f_Node.GetLeafNodes();
            foreach (var item in leafNodes)
            {
                var tempItem = item;
                var curNode = GetNode(tempItem.Value);
                if (curNode.IsActive)
                {
                    var list = GetDepthLoopFirstActive(curNode);
                    foreach (var node in list)
                    {
                        var tempNode = node;
                        nodes.TryAdd(tempNode.Key, tempNode.Value);
                    }
                }
                else
                {
                    nodes.TryAdd(tempItem.Value, curNode);
                }
            }
        }
        else
        {
            var lastNode = f_Node;
            var parentNode = GetParentNode(lastNode.Index);
            var isLoop = true;
            while (parentNode != null && isLoop)
            {
                if (parentNode.IsActive)
                {
                    isLoop = false;
                }
                else
                {
                    lastNode = parentNode;
                }
                parentNode = GetParentNode(parentNode.Index);
            }
            nodes.TryAdd(lastNode.Index, lastNode);
        }


        return nodes;
    }







    /// <summary>
    /// 更新一个节点状态
    /// </summary>
    /// <param name=""></param>
    public void UpdateNodeStatus(QuadTreeNode f_Node)
    {
        var curState = f_Node.IsActive;
        if (curState)
        {
            var parentNode = GetParentNode(f_Node.Index);
            while (parentNode != null)
            {
                parentNode.SetCurStatus(curState);
                parentNode = GetParentNode(parentNode.Index);
            }
        }
        else
        {
            var childNode = f_Node.GetLeafNodes();
            foreach (var item in childNode)
            {
                var tempItem = item;
                SetNodeStatus(tempItem.Value, curState);
                var nodes2 = GetNode(tempItem.Value);
                UpdateNodeStatus(nodes2);
            }
        }
    }



    /// <summary>
    /// 修改某个节点的状态
    /// </summary>
    /// <param name="f_Index"></param>
    /// <param name="f_ToState"></param>
    public void SetNodeStatus(QuadTreeNode f_Node, bool f_ToState)
    {
        f_Node.SetCurStatus(f_ToState);
    }

    /// <summary>
    /// 修改某个节点的状态
    /// </summary>
    /// <param name="f_Index"></param>
    /// <param name="f_ToState"></param>
    public void SetNodeStatus(int f_Index, bool f_ToState)
    {
        if (TreeNodeList.TryGetValue(f_Index, out var value))
        {
            SetNodeStatus(value, f_ToState);

            UpdateNodeStatus(value);
        }
    }

    #endregion











    #region shader 相关 

    /// <summary>
    /// 特效节点缓存
    /// </summary>
    public Dictionary<int, QuadTreeNode> TreeNodeEffectList = new();


    private Shader TargetShader => Shader.Find("Custom/AreaShader");
    public Material Mat_Range = null;
    public Material Mat_Edge = null;
    public Material Mat_Frame = null;
    public Material Mat_EnhanceEdge = null;
    public Material Mat_Blurred = null;
    public Material EdgeMat = null;


    /// <summary>
    /// 刷新 shader 参数
    /// </summary>
    public void UpdateShaderParams()
    {
        Mat_Range ??= new Material(Shader.Find("Custom/AreaShader"));
        EdgeMat ??= new Material(Shader.Find("Custom/EdgeShader"));
        Mat_Edge ??= new Material(Shader.Find($"Custom/EdgeFrameShader"));
        Mat_Frame ??= new Material(Shader.Find($"Custom/FrameShader"));
        Mat_EnhanceEdge ??= new Material(Shader.Find($"Custom/EnhanceEdgeShader"));
        Mat_Blurred ??= new Material(Shader.Find($"Custom/BlurredShader"));

        List<Vector4> arrParams = new();

        foreach (var item in TreeNodeEffectList)
        {
            var tempItem = item;
            var pos = tempItem.Value.Position;
            arrParams.Add(new Vector4(pos.x, pos.y, tempItem.Value.Radius, tempItem.Value.AreaBlend));
        }

        Mat_Range?.SetInteger("_ArrLength", arrParams.Count);
        Mat_Range?.SetVectorArray("_ArrPoint", arrParams.ToArray());
    }


    /// <summary>
    /// 更新所有节点的特效
    /// </summary>
    public void UpdateAllNodeEffect()
    {
        var effectList = GetDepthLoopFirstActive(RootNode);

        foreach (var item in TreeNodeEffectList)
        {
            item.Value.ChangeEffectStatus(false);
        }

        foreach (var item in effectList)
        {
            item.Value.ChangeEffectStatus(true);
        }

        TreeNodeEffectList = effectList;
    }



    /// <summary>
    /// 清空当前节点的子节点特效，不包含传入节点
    /// </summary>
    /// <param name="f_Node"></param>
    public void ClearChildEffect(QuadTreeNode f_Node)
    {
        var childNode = f_Node.GetLeafNodes();
        foreach (var item in childNode)
        {
            var tempItem = item;
            var node = GetNode(tempItem.Value);
            if (TreeNodeEffectList.TryGetValue(tempItem.Value, out var value))
            {
                node.ChangeEffectStatus(false);
                TreeNodeEffectList.Remove(tempItem.Value);
            }
            else
            {
                ClearChildEffect(node);
            }
        }
    }

    #endregion



    #region 迷雾功能
    RenderTexture m_RT = null;
    Texture2D m_CurMistyRange = null;

    public void StartRender(int f_Width, int f_Height)
    {
        // 范围
        RenderTexture rangeRT = RenderTexture.GetTemporary(f_Width, f_Height, 0);
        // 边缘
        RenderTexture edgeRT = RenderTexture.GetTemporary(f_Width, f_Height, 0);
        // 边缘扩张
        RenderTexture edgeEnhanceRT = RenderTexture.GetTemporary(f_Width, f_Height, 0);
        // 边缘虚化
        RenderTexture blurredRT = RenderTexture.GetTemporary(f_Width, f_Height, 0);

        {
            Graphics.Blit(null, rangeRT, Mat_Range);
            EdgeMat.SetTexture("_MainTex", rangeRT);
        }

        {
            Mat_Edge.SetTexture("_MainTex", rangeRT);
            Graphics.Blit(rangeRT, edgeRT, Mat_Edge);
        }
        {
            Mat_EnhanceEdge.SetTexture("_MainTex", edgeRT);

            Graphics.Blit(edgeRT, edgeEnhanceRT, Mat_EnhanceEdge);
        }
        {
            Mat_Blurred.SetTexture("_MainTex", edgeEnhanceRT);

            Graphics.Blit(edgeEnhanceRT, blurredRT, Mat_Blurred);
        }


        Mat_Frame.SetTexture("_RangeTex", rangeRT);
        Mat_Frame.SetTexture("_EdgeTex", blurredRT);




        {
            var texture2D = new Texture2D(f_Width, f_Height, TextureFormat.ARGB32, false);
            RenderTexture.active = rangeRT;
            texture2D.ReadPixels(new Rect(0, 0, f_Width, f_Height), 0, 0);
            texture2D.Apply();
            m_CurMistyRange = texture2D;
        }

    }

    public void StopRender()
    {
        Object.DestroyImmediate(m_CurMistyRange);
        RenderTexture.ReleaseTemporary(m_RT);
    }

    public void UpdateRangeData(Dictionary<int, QuadTreeNode> f_NodeList, int f_Width, int f_Height)
    {
        var canvasSizeDelta = new Vector2(f_Width, f_Height);
        List<Vector4> arrParams = new();

        foreach (var item in f_NodeList)
        {
            if (!item.Value.IsEffect) continue;
            var pos = item.Value.Position / canvasSizeDelta;
            //pos.y *= canvasSizeDelta.y / canvasSizeDelta.x;
            arrParams.Add(new Vector4(pos.x, pos.y, item.Value.Radius, item.Value.AreaBlend));
        }

        if (arrParams.Count > 0)
        {
            Mat_Range.SetInteger("_ArrLength", arrParams.Count);
            Mat_Range.SetVectorArray("_ArrPoint", arrParams.ToArray());
        }
    }


    public bool IsShow(Vector2Int f_LocalPos)
    {
        //var tex = Mat_Frame.GetTexture("_RangeTex");
        //var tex2D = tex as Texture2D;
        //var renTex2D = tex as RenderTexture;
        if (m_CurMistyRange == null)
        {
            return true;
        }
        else
        {
            //var curUV = f_LocalPos / new Vector2(m_CurMistyRange.width, m_CurMistyRange.height);


            var col = m_CurMistyRange.GetPixel(f_LocalPos.x, f_LocalPos.y);
            return col.a == 0;
        }
    }



    #endregion
}