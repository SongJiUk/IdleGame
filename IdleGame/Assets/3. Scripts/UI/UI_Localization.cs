using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Localization : MonoBehaviour
{
    public string LocalName;
    TextMeshProUGUI text;
    public string[] SemiData;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        SetLocalData();
    }

    public void SetLocalData()
    {
        if(LocalName != "")
        {
            string temp = "";
            if(SemiData.Length > 0)
            {
                temp = string.Format(Managers.LocalM.localData["UI" + LocalName].GetData(), SemiData);
            }
            else temp = Managers.LocalM.localData["UI" + LocalName].GetData();
            text.text = temp;
        }
    }
}
