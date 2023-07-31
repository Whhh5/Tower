using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class TrailRenderBase : Entity
{
    [SerializeField]
    private TrailRenderer m_TailRender = null;
    public override async UniTask OnLoadAsync()
    {
        await base.OnLoadAsync();
    }

    public override async UniTask OnStartAsync(params object[] f_Params)
    {
        m_TailRender.enabled = true;
    }

    public override async UniTask OnUnLoadAsync()
    {
        await base.OnUnLoadAsync();
        m_TailRender.Clear();
        m_TailRender.enabled = false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override async UniTask StartExecute()
    {

    }

    public override async UniTask StopExecute()
    {

    }

    public async UniTask WaitDestroyTimeAsync()
    {
        await UniTask.Delay((int)(m_TailRender.time * 1000));
    }
}
