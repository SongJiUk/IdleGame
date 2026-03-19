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
        if (text == null) text = GetComponent<TextMeshProUGUI>();
        if (text == null) return;

        if (LocalName != "")
        {
            string temp = "";
            if (SemiData.Length > 0)
            {
                temp = string.Format(Managers.LocalizationM.Get("UI" + LocalName), SemiData);
            }
            else temp = Managers.LocalizationM.Get("UI" + LocalName);
            text.text = temp;
        }
    }
}
