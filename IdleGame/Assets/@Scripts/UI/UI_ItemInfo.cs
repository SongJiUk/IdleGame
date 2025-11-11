using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class UI_ItemInfo : UI_Popup
{

    RectTransform rect;
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        rect = GetComponent<RectTransform>();
        return true;
    }


    //TODO : ���⼭ ��ġ �ʱ�ȭ ��Ű�� (�����Ͱ��� �޾ƿ;���)
    public void SetInfo(Vector2 _pos)
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();


        rect.pivot = PivotPoint(_pos);
        rect.anchoredPosition = _pos;


    }


}
