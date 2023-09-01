using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DependChunkData : EntityData
{
    public abstract ELayer LayerMask { get; }
    public abstract ELayer AttackLayerMask { get; }

    public WeaponBaseData CurWeaponData { get; protected set; }

    protected DependChunkData(int f_index, int f_ChunkIndex) : base(f_index)
    {
        CurrentIndex = f_ChunkIndex;
    }


    public int CurrentIndex { get; protected set; }
    public virtual int BranchOutIndex => 0;
    public List<int> AddtionIndexLiat = new();
    public void SetCurrentChunkIndex(int f_ToIndex)
    {
        CurrentIndex = f_ToIndex;
        AddtionIndexLiat.Clear();
        if (GTools.WorldMapMgr.TryGetRangeChunkByIndex(f_ToIndex, out var list, null, false, BranchOutIndex))
        {
            foreach (var item in list.GetEnumerator())
            {
                AddtionIndexLiat.Add(item.Value);
            }
        }
    }
    public bool TryExistChunk(int f_Index)
    {
        return CurrentIndex == f_Index || AddtionIndexLiat.Contains(f_Index);
    }
    public List<int> GetAllChunkIndex()
    {
        List<int> retult = new();
        retult.AddRange(AddtionIndexLiat);
        retult.Add(CurrentIndex);
        return retult;
    }
    public void ApplyCurrentChunk()
    {
        GTools.WorldMapMgr.MoveChunkElement(this, CurrentIndex);
    }
}


public abstract class DependChunk : Entity
{
    public Animator Animator => GetComponent<Animator>();



}