using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderManager : MonoBehaviour
{
    public UIRenderCharacter renderCharacter;

    private void Start()
    {
        if(Managers.Instance!=null)
        {
            Managers.Instance.SetRenderManager(this);
        }
        else
        {
            Debug.LogError("Managers 인스턴스가 없음");
        }
    }


}
