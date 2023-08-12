using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Button3D))]
public class WeaponBos3D : MonoBehaviour, IButton3DClick
{
    private Button3D m_Button => GetComponent<Button3D>();

    [SerializeField]
    private WeaponBase m_Target = null;


    [SerializeField]
    private EAssetName m_PrefabName;


    private async void Awake()
    {

        m_Button.AddListener(this);
        m_Button.AddListener2(this);


    }



    public async UniTask OnClickAsync()
    {

        //m_Target = await GTools.WeaponMgr.GetSetWeaponAsync(m_PrefabName, GTools.PlayerController.CurController);
        //await GTools.PlayerController.SetWeapon(m_Target);
    }

    public async UniTask OnClick2Async()
    {
        
    }
}
