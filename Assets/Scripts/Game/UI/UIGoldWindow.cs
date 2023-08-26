using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIGoldWindow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_GoldCount = null;
    
    public void UpdateGoldCount()
    {
        var value = GTools.PlayerMgr.CurGoldNum;
        m_GoldCount.text = $"{value}";
    }
}
