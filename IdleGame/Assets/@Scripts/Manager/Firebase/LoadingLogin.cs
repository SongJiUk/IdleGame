using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        DebugConsole.Instance.Log("GetLoingInit()");
        GuestLogin.onClick.AddListener(async () => await Managers.FirebaseM.GuestLogin());
        GoogleLogin.onClick.AddListener(() => Managers.FirebaseM.OnGoogleLoginClicked());
    }

    public void LoginComplete()
    {
        gameObject.SetActive(false);
    }
}
