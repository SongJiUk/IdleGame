using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class LoadingLogin : MonoBehaviour
{
    public static LoadingLogin instance = null;


    public Button GuestLogin;
    public Button GoogleLogin;

    private void Awake()
    {
        if (instance == null) instance = this;
        gameObject.SetActive(true);

    }

    public void GetLoginInit()
    {
        GuestLogin.onClick.AddListener(async () => await Managers.FirebaseM.GuestLogin());
        GoogleLogin.onClick.AddListener(() => Managers.FirebaseM.GoogleLogin());
    }

    public void LoginComplete()
    {
        gameObject.SetActive(false);
    }
}
