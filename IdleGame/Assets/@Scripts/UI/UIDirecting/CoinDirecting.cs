using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CoinDirecting : UIDirecting
{
    Vector3 targetPos;
    Camera cam;
    RectTransform[] childs = new RectTransform[5];
    float distanceRange;


    //DoTween ���
    const float SCATTER_DURATION = 0.3f;
    const float COLLECT_DURATION = 0.3f;
    private void Awake()
    {
        cam = Camera.main;
        for (int i = 0; i < childs.Length; i++) childs[i] = transform.GetChild(i).GetComponent<RectTransform>();
        distanceRange = 300f;

    }

    #region  Unitask
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
    public override bool Init()
    {
        if (!base.Init()) return false;
        return true;
    }
    public void Init(Vector3 _pos)
    {
        if ((Managers.UIM.SceneUI as UI_GameScene).isSavingMode)
        {
            Managers.GameM.Gold += Utils.Money();


            if (this != null && gameObject != null)
                Managers.ObjectM.DeSpawn(this);

            return;
        }
        targetPos = _pos;

        if (cam != null) transform.position = cam.WorldToScreenPoint(targetPos);
        for (int i = 0; i < childs.Length; i++) childs[i].anchoredPosition = Vector2.zero;
        transform.SetParent(Managers.UIM.SceneUI.WorldCoinParent, false);

        Managers.GameM.Gold += Utils.Money();


        Coin_Effect_DoTween(cts.Token).Forget();
        //Coin_Effect_DoTween(this.GetCancellationTokenOnDestroy()).Forget();

    }

    public async UniTaskVoid Coin_Effect_DoTween(CancellationToken _token)
    {
        try
        {
            for (int i = 0; i < childs.Length; i++)
            {
                RectTransform rect = childs[i];
                Vector2 randOffset = (Vector2)targetPos + Random.insideUnitCircle * Random.Range(-distanceRange, distanceRange);

                rect.DOAnchorPos(randOffset, SCATTER_DURATION)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true) // �̰� ���� �Ͻ������Ǿ ��� ������
                    .SetDelay(Random.Range(0f, 0.05f))
                    .ToUniTask(cancellationToken: _token)
                    .Forget();
            }

            //�̸���
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

            if (this != null && gameObject != null)
                Managers.ObjectM.DeSpawn(this);
        }
        catch (System.OperationCanceledException)
        {
            if (this != null && gameObject != null)
                Managers.ObjectM.DeSpawn(this);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            if (this != null && gameObject != null)
                Managers.ObjectM.DeSpawn(this);
        }
    }


}
