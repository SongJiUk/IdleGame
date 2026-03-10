using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CameraManager
{
    CameraController cc;

    public void SetController(CameraController _cc)
    {
        if (_cc != null) cc = _cc;
    }

    public void Clear()
    {
        cc = null;
        Debug.Log("[CameraManager] 카메라 참조 초기화 완료");
    }
    public async UniTask CameraShake()
    {

        if (cc == null) return;
        if (PlayerPrefs.GetInt("CAM") == 1) return;

        if (cc.isCameraShake) return;
        cc.isCameraShake = true;


        CancellationToken ct = cc.cancellationToken;
        if (ct.IsCancellationRequested) return;

        float timer = 0f;
        while (timer < cc.Duration)
        {
            if (ct.IsCancellationRequested) return;

            cc.transform.localPosition = Random.insideUnitSphere * cc.Power + cc.OriginPos;

            timer += Time.deltaTime;
            await UniTask.Yield(ct);
        }


        if (!ct.IsCancellationRequested)
            cc.transform.localPosition = cc.OriginPos;

        cc.isCameraShake = false;
    }

}
