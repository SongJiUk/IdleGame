using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseScene : MonoBehaviour
{
    private void Awake()
    {
        Init();
    }

    public virtual void Init()
    {
        var obj = FindObjectOfType(typeof(EventSystem));

        if (obj == null)
            Managers.ResourceM.Instantiate("UI/Eventsystem").name = "@EventSystem";
    }
}
