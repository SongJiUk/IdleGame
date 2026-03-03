using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IAP_LocalText : MonoBehaviour
{
    public string productID;

    private void Start()
    {
        InitIAPText();
    }

    void InitIAPText()
    {
#if UNITY_EDITOR || UNITY_ANDROID
        GetComponent<TextMeshProUGUI>().text = string.Format("{0} {1}",
            Managers.IAPM.GetProduct(productID).metadata.localizedPrice,
            Managers.IAPM.GetProduct(productID).metadata.isoCurrencyCode);
#endif
    }
}
