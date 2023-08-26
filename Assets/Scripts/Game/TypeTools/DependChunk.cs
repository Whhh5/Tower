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
    public void SetCurrentChunkIndex(int f_ToIndex)
    {
        CurrentIndex = f_ToIndex;
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