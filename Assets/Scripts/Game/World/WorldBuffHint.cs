using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuffHintData : UnityObjectData
{
    public WorldBuffHintData(int f_Index) : base(f_Index)
    {
    }

    public override AssetKey AssetPrefabID => AssetKey.Chunk1;

    public override void AfterLoad()
    {
    }

    public override EWorldObjectType ObjectType => EWorldObjectType.None;
}
public class WorldBuffHint : ObjectPoolBase
{
    [SerializeField]
    private BuffBase m_Target = null;
    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();

    }

    public override async UniTask OnStartAsync(params object[] f_Params)
    {

    }

    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
        m_Target = null;
    }

    public override void OnUpdate()
    {

    }


    public async UniTask SetTargetAsync(BuffBase f_Target)
    {
        m_Target = f_Target;
    }



    private void OnMouseEnter()
    {
        Log($"{m_Target?.Name}   Enter");
    }

    private void OnMouseExit()
    {
        Log($"{m_Target?.Name}   Exit");
    }
}
