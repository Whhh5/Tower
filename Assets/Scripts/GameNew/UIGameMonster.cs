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
    [SerializeField]
    private Image m_MainSlider = null;
    [SerializeField]
    private Image m_MainSliderColor = null;
    [SerializeField]
    private Color m_FormColor;
    [SerializeField]
    private Color m_ToColor;
    private float m_CurSliderValue = 0.0f;
    public override async UniTask AwakeAsync()
    {
        m_CurSliderValue = 0.0f;
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
    private float m_TempSlider;
    protected override void Update()
    {
        base.Update();

        if (GTools.CreateMapNew.TryGetNextWaveTime(out var value))
        {
            var residueTime = value - Time.time;
            m_NextWaveResidueTimeTxt.text = Mathf.Max(0, residueTime).ToString("0");
            var curWaveTime = GTools.CreateMapNew.GetCurWaveTime();
            var showTime = curWaveTime / 1.5f;
            m_ActiveBtn.gameObject.SetActive(false);
            if (residueTime < 0)
            {
                NextWave();
            }
            else if (residueTime < showTime)
            {
                m_ActiveBtn.gameObject.SetActive(true);
            }

            m_TempSlider = residueTime / curWaveTime;
            var slider = Mathf.Lerp(m_CurSliderValue, m_TempSlider, Time.deltaTime * 2.0f);
            SetResidueTimeSlider(slider);

        }
        else
        {
            m_NextWaveResidueTimeTxt.gameObject.SetActive(false);
            m_ActiveBtn.gameObject.SetActive(false);
            SetResidueTimeSlider(0);
        }

    }

    private void SetResidueTimeSlider(float f_SliderValue)
    {
        var color = Color.Lerp(m_ToColor, m_FormColor, f_SliderValue);
        m_MainSlider.fillAmount = f_SliderValue;
        m_MainSliderColor.color = color;
        m_CurSliderValue = f_SliderValue;
    }
}
