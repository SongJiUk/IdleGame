using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public Action OnClickHandler = null;
    public Action OnPressHandler = null;
    public Action OnPointerDownHandler = null;
    public Action OnPointerUpHandler = null;
    public Action<BaseEventData> OnDragHandler = null;
    public Action<BaseEventData> OnBeginDragHandler = null;
    public Action<BaseEventData> OnEndDragHandler = null;

    bool isPressed = false;


    void Update()
    {
        if (isPressed) OnPressHandler?.Invoke();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragHandler?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        isPressed = true;
        OnDragHandler?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isPressed = false;
        OnEndDragHandler?.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null) OnClickHandler?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        OnPointerDownHandler?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        OnPointerUpHandler?.Invoke();
    }
}
