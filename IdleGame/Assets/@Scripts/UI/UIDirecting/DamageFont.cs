using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageFont : BaseController
{
    [SerializeField]
    TextMeshProUGUI damageText;
    [SerializeField]
    GameObject criticalObject;
    Camera cam;
    Vector3 target;



    public void Init(Vector3 _pos, double _dmg, bool _isMonster = false, bool _isCritical = false, bool _isSkill = false)
    {
        if ((Managers.UIM.SceneUI as UI_GameScene).isSavingMode)
        {
            Managers.ResourceM.Destroy(gameObject);
            return;
        }

        cam = Camera.main;
        transform.SetParent(Managers.UIM.SceneUI.WorldFontParent, false);
        _pos.x += Random.Range(-0.3f, 0.3f);
        _pos.z += Random.Range(-0.3f, 0.3f);
        target = _pos;
        criticalObject.SetActive(false);

        if (_isMonster)
        {
            damageText.color = Utils.HexToColor("#FF0000");
            damageText.text = Utils.ToCurrencyString(_dmg);
        }
        else
        {
            if (_isCritical)
            {
                criticalObject.SetActive(true);
                damageText.color = Utils.HexToColor("#FFFFFF");
                damageText.text = Utils.ToCurrencyString(_dmg);
            }
            else
            {
                criticalObject.SetActive(false);
                damageText.color = Utils.HexToColor("#FFFFFF");
                damageText.text = Utils.ToCurrencyString(_dmg);
            }

            if (_isSkill)
            {
                criticalObject.SetActive(false);
                damageText.color = Utils.HexToColor("#0000FF");
                damageText.text = Utils.ToCurrencyString(_dmg);
            }
        }




        damageText.alpha = 1;

        DoAnim();

    }

    void DoAnim()
    {
        var tr = transform;
        Vector3 targetPos = new Vector3(target.x, target.y + 0.5f, target.z);
        if (cam != null) transform.position = cam.WorldToScreenPoint(targetPos);

        Sequence sq = DOTween.Sequence();
        transform.localScale = Vector3.zero;

        sq.Append(transform.DOScale(1.3f, 0.3f).SetEase(Ease.InOutBounce))
            .Join(transform.DOMove(transform.position + Vector3.up, 0.3f).SetEase(Ease.Linear))
            .Append(transform.DOScale(1.0f, 0.3f).SetEase(Ease.InOutBounce))
            .Join(damageText.DOFade(0, 0.3f).SetEase(Ease.InQuint))
            .OnComplete(() =>
            {
                Managers.ResourceM.Destroy(gameObject);
            });
    }
}
