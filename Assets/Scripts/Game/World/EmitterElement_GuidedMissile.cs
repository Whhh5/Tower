using B1;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitterElement_GuidedMissile : WeaponElementBase
{
    [SerializeField]
    private TrailRender_Common m_Trail = null;
    [SerializeField]
    Dictionary<ushort, ushort> m_BuffList = new()
    {
        { 1, 2 },
        { 2, 1 },
    };

    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
        m_Trail = await GTools.LoadTrailRenderAsync<TrailRender_Common>(TrailPoint.position);
    }

    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        
    }

    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
        await GTools.UnLoadTrailRenderAsync(m_Trail);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }


    public override void SetWorldPos(Vector3 f_WorldPos)
    {
        base.SetWorldPos(f_WorldPos);
        m_Trail.SetWorldPos(TrailPoint.position);
    }


    public override async UniTask StopExecute()
    {
        await  base.StopExecute();

        var targets = GTools.MathfMgr.GetTargets_Sphere(CentralPoint.position, 10, ELayer.Enemy);

        await GTools.ParallelTaskAsync(targets, async (entity) =>
        {
            await entity.BeHitAsync(m_Harm, EDamageType.PhCritical);

            await GTools.ParallelTaskAsync(m_BuffList, async (key, value) =>
            {
                await (entity as Person).AddBuffAsync(key, value);
            });
        });

        await AssetsMgr.Ins.UnLoadPrefabPoolAsync(this);
    }
}
