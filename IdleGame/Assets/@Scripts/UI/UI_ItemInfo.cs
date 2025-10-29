using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_ItemInfo : UI_Popup
{

    RectTransform rect;
    public override bool Init()
    {
        if (!base.Init()) return false;

        rect = GetComponent<RectTransform>();
        return true;
    }


    //TODO : 여기서 위치 초기화 시키기 (데이터값도 받아와야함)
    public void SetInfo(Vector2 _pos)
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();


        rect.pivot = PivotPoint(_pos);
        rect.anchoredPosition = _pos;


    }

  
}
