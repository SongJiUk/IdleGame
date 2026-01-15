using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class UI_Scene : UI_Base
{
    public abstract Transform WorldFontParent { get; }
    public abstract Transform WorldCoinParent { get; }
    public abstract Transform WorldJewelParent { get; }
    public abstract Transform WorldItemParent { get; }
    public abstract Transform WorldSpeechParent { get; }

    protected RectTransform coinDirectingTr;
    protected RectTransform jewelDirectingTr;
    protected Transform layers;
    protected Transform speechLayer;
    protected Transform popupLayer;

    public RectTransform CoinDirectingTr
    {
        get { return coinDirectingTr; }
    }

    public RectTransform CoinDirectingRectTr
    {
        get { return coinDirectingTr; }
    }
    public RectTransform JewelDirectingTr
    {
        get { return jewelDirectingTr; }
    }

    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        return true;
    }

    public Transform GetLayer(int _num)
    {
        if (layers == null)
        {
            Debug.LogError("layers가 없어요..!!");
            return null;
        }
        return layers.GetChild(_num);
    }
    
    public Transform GetSpeechLayer()
    {
        if(speechLayer == null)
        {
            Debug.LogError("SpeechLayer가 없음");
            return null;
        }
        return speechLayer.GetChild(0);
    }

    public Transform GetPopUpLayer()
    {
        if (popupLayer == null)
        {
            Debug.LogError("PopupLayer가 없음");
            return null;
        }

        return popupLayer;
    }
}
