using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B1;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class CameraManager : MonoSingleton<CameraManager>
{
    private Vector3 StartPosition => new Vector3(13, 6, -100);
    private float MoveTime => 5.0f;
    public async void StartMove()
    {
        var curWaveTime = GTools.CreateMapNew.GetCurWaveTime();
        var moveTime = curWaveTime / 4;

        var time1 = moveTime * 0.3f;
        var time2 = moveTime * 0.4f;
        var time3 = moveTime * 0.3f;


        var pos = StartPosition;
        var endPosX = StartPosition.x;
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
            endPosX = Mathf.Max(objData.WorldPosition.x, endPosX);
        }
        await DOTween.To(() => 0.0f, slider =>
          {
              var posX = Mathf.Lerp(StartPosition.x, endPosX, slider);
              pos.x = posX;
              transform.position = pos;

          }, 1.0f, time1)
            .SetEase(Ease.InOutSine);
        await UniTask.Delay(Mathf.FloorToInt(time2 * 1000));
        await DOTween.To(() => 0.0f, slider =>
        {
            var posX = Mathf.Lerp(endPosX, StartPosition.x, slider);
            pos.x = posX;
            transform.position = pos;

        }, 1.0f, time3)
            .SetEase(Ease.InOutSine);
        GTools.HeroCardPoolMgr.StartShowElement();
    }
}
