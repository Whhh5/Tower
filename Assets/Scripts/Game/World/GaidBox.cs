using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GaidBox : ObjectPoolBase
{
    private string m_TargetModleName = "";

    [SerializeField, Range(0.5f, 2.0f)]
    private float f_Radius = 1.0f;
    [SerializeField]
    private ELayer m_LayerMask = ELayer.Default;
    [SerializeField]
    List<uint> m_GainList = new();
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
    }

    public override void OnUpdate()
    {
        var nearbyTarget = GTools.MathfMgr.GetTargets_Sphere<Person>(Position, f_Radius, m_LayerMask, (person) =>
        {
            return GTools.PlayerController.CurController == person;
        });
    }
}
