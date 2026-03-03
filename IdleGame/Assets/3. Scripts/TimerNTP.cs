using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TimerNTP : MonoBehaviour
{
    //인터넷 시간 활용
    private static long diffTicks = 0;
    private static DateTime syncTime = DateTime.Now;
    public static DateTime NowTime => DateTime.Now.AddTicks(diffTicks);

    public static void UpdateDateTime(string _dataStr)
    {
        if(DateTime.TryParse(_dataStr, out DateTime serverTime))
        {
            syncTime = serverTime;
            //서버에서 전달받은 시간에서 현재 시간을 뺌(기기를 조정했다면 차이가 심함)
            diffTicks = (syncTime - DateTime.Now).Ticks;
        }
    }
}

public class TimeManager
{
    public float currentTime;
    public TimeSpan loginSpan;
    public Action<DateTime> OnTimeUpdated;
    private static bool isChecin = false;

    private const string serverUrl = "https://www.google.com";


    public void Init()
    {
        Managers.Instance.StartCoroutine(CheckServerTime());
    }

    public IEnumerator CheckServerTime()
    {
        //데이터 요청
        using UnityWebRequest request = UnityWebRequest.Get(serverUrl);
        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Network Error");
            //TODO : 연결 안되면 게임 종료(팝업창 하나 생성)
            Application.Quit();
        }
        else
        {
            string serverData = request.GetResponseHeader("Date");
            if(DateTime.TryParse(serverData, out DateTime serverTime))
            {
                Debug.Log("Server Time : " + serverTime);
                TimerNTP.UpdateDateTime(serverData);
            }
            else
            {
                Debug.Log("형변환 안됌");
            }
        }
    }
}
