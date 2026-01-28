using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class GoodsDirecting : UIDirecting
{
    Vector3 targetPos;
    Camera cam;
    [SerializeField] RectTransform[] childs = new RectTransform[5];
    float distanceRange;

    Define.GoodsType type;
    //DoTween ���
    const float SCATTER_DURATION = 0.3f;
    const float COLLECT_DURATION = 0.3f;
    private void Awake()
    {
        cam = Camera.main;
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
    public void Init(Define.GoodsType _type, Vector3 _pos, double _count, bool _isWorldPos = true)
    {
        type = _type;
        if ((Managers.UIM.SceneUI as UI_GameScene).isSavingMode)
        {
            Managers.GameM.Gold += Utils.Money();

            if (this != null && gameObject != null)
                Managers.ObjectM.DeSpawn(this);

            return;
        }

        targetPos = _pos;
        transform.SetParent(Managers.UIM.SceneUI.WorldGoodsParent, false);

        if (_isWorldPos)
        {
            if (cam != null) transform.position = cam.WorldToScreenPoint(targetPos);
        }
        else
        {
            transform.position = targetPos;
        }


        for (int i = 0; i < childs.Length; i++) childs[i].anchoredPosition = Vector2.zero;

        switch (type)
        {
            case Define.GoodsType.Gold:
                Managers.GameM.Gold += _count;
                break;
            case Define.GoodsType.Dia:
                Managers.GameM.Dia += _count;
                break;

        }

        Coin_Effect_DoTween(cts.Token).Forget();
    }

    public async UniTaskVoid Coin_Effect_DoTween(CancellationToken _token)
    {
        try
        {

            Vector3 destinationPos = type == Define.GoodsType.Gold
                ? Managers.UIM.SceneUI.CoinDirectingTr.position
                : Managers.UIM.SceneUI.DiaDirectingTr.position;

            Sprite symbolSprite = Managers.ResourceM.GetAtlas(type.ToString());


            for (int i = 0; i < childs.Length; i++)
            {
                if (childs[i] == null) continue;

                RectTransform rect = childs[i];
                rect.anchoredPosition = Vector2.zero;


                if (rect.TryGetComponent<Image>(out var img)) img.sprite = symbolSprite;

                Vector2 scatterPos = Random.insideUnitCircle * distanceRange;

                rect.DOAnchorPos(scatterPos, SCATTER_DURATION)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true)
                    .SetDelay(Random.Range(0f, 0.05f))
                    .ToUniTask(cancellationToken: _token)
                    .Forget();
            }

            await UniTask.Delay(500, cancellationToken: _token);

            var tasks = new UniTask[childs.Length];
            for (int i = 0; i < childs.Length; i++)
            {
                if (childs[i] == null) { tasks[i] = UniTask.CompletedTask; continue; }

                tasks[i] = childs[i].DOMove(destinationPos, COLLECT_DURATION)
                                   .SetEase(Ease.InCubic)
                                   .SetUpdate(true)
                                   .ToUniTask(cancellationToken: _token);
            }

            await UniTask.WhenAll(tasks);

            if (this != null) Managers.ObjectM.DeSpawn(this);
        }
        catch (System.OperationCanceledException) { /* 무시 */ }
        catch (System.Exception e) { Debug.LogError(e.Message); }
    }


}
