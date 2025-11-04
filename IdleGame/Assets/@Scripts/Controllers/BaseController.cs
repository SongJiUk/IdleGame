using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BaseController : MonoBehaviour, ITickable
{
    bool isInit = false;
    public virtual bool Init()
    {
        if (isInit) return false;

        isInit = true;
        return true;
    }

    public virtual void Tick(float _deltaTime)
    {

    }

    //TODO: 이걸 이렇게 사용해도 되나?
    public virtual async UniTask ReturnObject(float _time)
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();
        try
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(_time), cancellationToken: token);
            
        }
        catch (System.OperationCanceledException) { }
        catch (System.Exception e)
        {
            Debug.LogError($"Projectile 반환중 에러 발생 {e.Message}");
        }
    }
}
