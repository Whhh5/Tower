using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using B1.UI;
using Cysharp.Threading.Tasks;
using B1.Event;

public class UIAppPlanePage : UIWindowPage, IEventSystem
{
    protected override EAssetName SpriteAltas => EAssetName.None;

    public void ReceptionEvent(EEventSystemType f_Event, object f_Param, string f_SubDesc, string f_SendDesc)
    {
        switch (f_Event)
        {
            case EEventSystemType.None:
                break;
            case EEventSystemType.UI_WINDOW_LOAD_FINISH:
                Log($"{f_Param}    {f_SubDesc}  {f_SendDesc}");
                break;
            default:
                break;
        }
    }

    public Dictionary<EEventSystemType, string> SubscribeList()
    {
        return new()
        {
            {
                EEventSystemType.UI_WINDOW_LOAD_FINISH, "UIAppPlanePage"
            },
        };
    }

    protected override List<EAssetName> GetWindowNameAsync()
    {
        return new List<EAssetName>()
        {
            EAssetName.UIAppPlane,
        };
    }
}
