using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DependChunkData : EntityData
{
    public abstract ELayer LayerMask { get; }
    public abstract ELayer AttackLayerMask { get; }

    public WeaponBaseData CurWeaponData { get; protected set; }

    protected DependChunkData() : base(0)
    {
        
    }


    public int CurrentIndex { get; protected set; }
    public virtual int BranchOutIndex => 0;
    public void SetCurrentChunkIndex(int f_ToIndex)
    {
        CurrentIndex = f_ToIndex;
    }

    public virtual void InitData(int f_ChunkIndex = -1)
    {
        CurrentIndex = -1;
        if (f_ChunkIndex >= 0)
        {
            if (!this.MoveToChunk(f_ChunkIndex))
            {
                LogError($"ÒÆ¶¯Ê§°Ü index = {f_ChunkIndex}");
            }
            else if (GTools.CreateMapNew.TryGetChunkData(f_ChunkIndex, out var chunkData))
            {
                SetPosition(chunkData.WorldPosition);
            }
        }
    }
    public bool TryExistChunk(int f_Index)
    {
        return CurrentIndex == f_Index;
    }
    public List<int> GetAllChunkIndex()
    {
        List<int> retult = new();
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