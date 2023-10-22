using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using B1.UI;

public class GameMgr : MonoSingleton<GameMgr>
{
    public Transform PoolRoot = null;
    public Transform TestRoot = null;
    public Canvas Canvas = null;
    protected override void Awake()
    {
        base.Awake();
        GameObject.DontDestroyOnLoad(this);
    }

    protected override async void Start()
    {

        //await UIWindowManager.Ins.OpenPageAsync<UIAppPlanePage>();
        //await UIWindowManager.Ins.OpenPageAsync<UINavigationBarPage>();


        // ����һ�� page
        //await UIWindowManager.Ins.OpenPageAsync<UILobbyPage>();
        // �ر�һ�� page
        //await UIWindowManager.Ins.ClosePageAsync<UILobbyPage>();
    }
}
