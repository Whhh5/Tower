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
    Txt_Describes,

    Tran_Icon1,
    Tran_Icon2,
    Tran_Point,

    Item_Point,


    Img_Quality,
    Img_Icon,
    Img_Icon1,
    Img_Icon2,
    Img_Vocational,
    Img_Slider,

    Btn_Click,
    Btn_Close,

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
