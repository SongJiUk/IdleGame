using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CoinDirecting : MonoBehaviour
{
    Vector3 targetPos;
    Camera cam;
    RectTransform[] childs = new RectTransform[5];
    float distanceRange;

    
    //DoTween 상수
    const float SCATTER_DURATION = 0.3f;
    const float COLLECT_DURATION = 0.3f;
    private void Awake()
    {
        cam = Camera.main;
        for (int i = 0; i < childs.Length; i++) childs[i] = transform.GetChild(i).GetComponent<RectTransform>();
        distanceRange = 300f;
       
    }

    #region 이거 Unitask돌아갈때 게임종료하면 나는 오류때문에 작성해놓은거
    private CancellationTokenSource cts;
    private void OnEnable()
    {
        cts = new CancellationTokenSource();
    }
    private void OnDisable()
    {
        cts?.Cancel();
        cts.Dispose();
        cts = null;
    }
    #endregion
    //TODO: 데이터 추가하기
    public void Init(Vector3 _pos)
    {
        targetPos = _pos;

        transform.position = cam.WorldToScreenPoint(targetPos);
        for (int i = 0; i < childs.Length; i++) childs[i].anchoredPosition = Vector2.zero;
        transform.parent = Managers.UIM.SceneUI.WorldCoinParent;

        Coin_Effect_DoTween(cts.Token).Forget();
        //Coin_Effect_DoTween(this.GetCancellationTokenOnDestroy()).Forget();


    }

    public async UniTaskVoid Coin_Effect_DoTween(CancellationToken _token)
    {
        try
        {
            for(int i = 0; i <childs.Length; i++)
            {
                RectTransform rect = childs[i];
                Vector2 randOffset = (Vector2)targetPos + Random.insideUnitCircle * Random.Range(-distanceRange, distanceRange);

                rect.DOAnchorPos(randOffset, SCATTER_DURATION)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true) // 이거 쓰면 일시정지되어도 계속 진행됌
                    .SetDelay(Random.Range(0f, 0.05f))
                    .ToUniTask(cancellationToken: _token)
                    .Forget();
            }

            //미리초
            await UniTask.Delay(500, cancellationToken: _token); 

            var tasks = new UniTask[childs.Length];
            for (int i = 0; i < childs.Length; i++)
            {
                RectTransform rect = childs[i];
                tasks[i] = rect.DOMove(Managers.UIM.SceneUI.CoinDirectingTr.position, COLLECT_DURATION)
                              .SetEase(Ease.InCubic)
                              .SetUpdate(true)
                              .ToUniTask(cancellationToken: _token);
            }

            await UniTask.WhenAll(tasks);

            if(this != null && gameObject != null)
               Managers.ResourceM.Destory(gameObject);
        }
        catch(System.OperationCanceledException)
        {
            if (this != null && gameObject != null)
                Managers.ResourceM.Destory(gameObject);
        }
        catch(System.Exception e)
        {
            Debug.Log("코인 이펙트 취소 :" + e.Message);
            if (this != null && gameObject != null)
                Managers.ResourceM.Destory(gameObject);
        }
    }

    
}
