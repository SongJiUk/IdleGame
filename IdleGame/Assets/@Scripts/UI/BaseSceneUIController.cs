using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaseSceneUIController : MonoBehaviour
{
    Button buttonType;
    Button buttonComponent;
    TextMeshProUGUI textComponent;
    Image imageComponent;


    private void Awake()
    {
        if (buttonComponent == null)
            buttonComponent = GetComponent<Button>();

        if (textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();

        if (imageComponent == null)
            imageComponent = GetComponent<Image>();

        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(HandleClick);
        }

        if (textComponent != null)
        {

        }

    }

    void HandleClick()
    {

    }
}
