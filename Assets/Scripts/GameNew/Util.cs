using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public enum EChildName
{
    Txt_Title,
    Txt_Name,
    Txt_Level,
    Txt_Count,
    Txt_GoldCount,

    Tran_Icon1,
    Tran_Icon2,

    Img_Quality,
    Img_Icon,

    Btn_Click,

}
public static class Util
{
    public static TCom GetChildCom<TCom>(this Transform f_Prant, EChildName f_Name)
        where TCom : Component
    {
        var child = f_Prant.Find(f_Name.ToString());
        TCom com = null;
        if (child != null)
        {
            com = child.GetComponent<TCom>();
        }
        return com;
    }
}
