using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;

public class CameraManager : MonoSingleton<CameraManager>
{
    private Vector3 StartPosition => new Vector3(12, 6, -100);
    public void StartMove()
    {
        if (!GTools.CreateMapNew.TryGetAllObject(ELayer.Enemy, out var list))
        {
            return;
        }
        foreach (var item in list)
        {
            if (item is not WorldObjectBaseData objData)
            {
                continue;
            }

        }
    }
}
