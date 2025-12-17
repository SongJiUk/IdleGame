using Firebase.Auth;
using UnityEngine;

public partial class FirebaseManager
{
    //Guest
    public void GuestLogin()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            //if(auth.CurrentUser != null)
            //{
            //    Debug.Log("기기에 로그인된 상황입니다. : " + auth.CurrentUser.UserId);
            //    ReadData();
            //    return;
            //}

            if(task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("게스트 로그인 실패");
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log("게스트 로그인 성공 ! 사용자 ID : " + user.UserId);
            ReadData();
        });
    }
    
}
