using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;

public class UI_Toast : UI_Base
{
    enum Images
    {
        BackgroundImage
    }
    enum Texts
    {
        ToastMessageValueText
    }

    private float moveDistance = 100f;
    private float duration = 1.0f;
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        ImagesType = typeof(Images);
        TextsType = typeof(Texts);

        BindImage(ImagesType);
        BindText(TextsType);


        return true;
    }


    public void SetInfo(string _detail)
    {
        transform.SetAsLastSibling();
        var text = GetText(TextsType, (int)Texts.ToastMessageValueText);
        var image = GetImage(ImagesType, (int)Images.BackgroundImage);

        text.text = _detail;

        text.DOKill();
        image.DOKill();

        Color textColor = text.color;
        textColor.a = 1f;
        text.color = textColor;

        Color imageColor = image.color;
        imageColor.a = 1f;
        image.color = imageColor;


        float duration = 1.0f; // 전체 연출 시간

        image.DOFade(0f, duration).SetEase(Ease.InQuad);

        text.DOFade(0f, duration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                Managers.UIM.CloseToast(this);
            });

    }

}
