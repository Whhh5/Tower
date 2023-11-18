using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class UIGameFinish : UIWindow
{
    private bool m_IsFinish = false;
    private float m_LastDetectionTime = 0.0f;
    private float m_DetectionInterval = 2.0f;
    [SerializeField]
    private Transform m_MainBody = null;
    [SerializeField]
    private Button m_ReturnBtn = null;
    [SerializeField]
    private Transform m_VictoryTran = null;
    [SerializeField]
    private Transform m_DefeatedTran = null;
    public override async UniTask AwakeAsync()
    {
        m_ReturnBtn.onClick.RemoveAllListeners();
        m_ReturnBtn.onClick.AddListener(() =>
        {
            GameManager.ReturnSelectWindow();
        });
        m_IsFinish = false;

        m_MainBody.gameObject.SetActive(false);
    }

    public override async UniTask OnShowAsync()
    {



    }

    protected override void Update()
    {
        if (m_IsFinish)
        {
            return;
        }
        base.Update();

        if (Time.time - m_LastDetectionTime < m_DetectionInterval)
        {
            return;
        }

        var isVictory = false;
        var isFinish = false;
        var curActiveEnergy = GTools.CreateMapNew.GetCurActiveEnergyCount();
        if (curActiveEnergy <= 0)
        {
            isVictory = true;
            isFinish = true;
        }
        var curWave = GTools.CreateMapNew.GetCurWaveCount();
        var maxWave = GTools.CreateMapNew.GetMaxWaveCount();
        var curMonsterCount = GTools.CreateMapNew.GetCurWaveMonsterActiveCount();
        if (curWave + 1 == maxWave && curMonsterCount <= 0)
        {
            isVictory = false;
            isFinish = true;
        }
        if (!isFinish)
        {
            return;
        }

        m_IsFinish = true;
        m_VictoryTran.gameObject.SetActive(!isVictory);
        m_DefeatedTran.gameObject.SetActive(isVictory);
        m_MainBody.gameObject.SetActive(true);
        if (!isVictory)
        {
            GameDataMgr.Ins.PassCurLevel();
        }
    }
}
