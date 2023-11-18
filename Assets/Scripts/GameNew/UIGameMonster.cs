using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class UIGameMonster : UIWindow
{
    [SerializeField]
    private Button m_ActiveBtn = null;
    [SerializeField]
    private TextMeshProUGUI m_NextWaveResidueTimeTxt = null;
    public override async UniTask AwakeAsync()
    {
        m_ActiveBtn.onClick.RemoveAllListeners();
        m_ActiveBtn.onClick.AddListener(() =>
        {
            if (GTools.CreateMapNew.TryGetNextWaveTime(out var value))
            {
                var residueTime = value - Time.time;

                GTools.PlayerMgr.Increases(Mathf.CeilToInt(residueTime * 0.8f));
            }
            NextWave();
        });
    }

    public override async UniTask OnShowAsync()
    {

    }
    private void NextWave()
    {
        if (GTools.CreateMapNew.NextWave())
        {
            GTools.CreateMapNew.SetWaveMonsterActive();
        }
    }

    private float m_LastWaveTime = 0.0f;
    protected override void Update()
    {
        base.Update();

        if (GTools.CreateMapNew.TryGetNextWaveTime(out var value))
        {
            var residueTime = value - Time.time;
            m_NextWaveResidueTimeTxt.text = Mathf.Max(0, residueTime).ToString("0");
            var showTime = GTools.CreateMapNew.GetCurWaveTime() / 1.5f;
            m_ActiveBtn.gameObject.SetActive(false);
            if (residueTime < 0)
            {
                NextWave();
            }
            else if (residueTime < showTime)
            {
                m_ActiveBtn.gameObject.SetActive(true);
            }
        }
        else
        {
            m_NextWaveResidueTimeTxt.gameObject.SetActive(false);
            m_ActiveBtn.gameObject.SetActive(false);
        }

    }
}
