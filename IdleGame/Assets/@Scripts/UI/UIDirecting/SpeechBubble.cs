using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Cysharp.Threading.Tasks;

public class SpeechBubble : UIDirecting
{
    [SerializeField] TextMeshProUGUI text;
    private readonly Vector3 PosOffset = new Vector3(0, 0.9f, 0);
    Vector3 pos;
    Camera cam;
    Data.NPCData data;
    RectTransform myRect;
    RectTransform parentRect;

    private bool isTextEnabled = true;

    public override bool Init()
    {
        if (!base.Init()) return false;
        myRect = GetComponent<RectTransform>();
        return true;
    }

    public void Init(Vector3 _pos, Camera _cam, Data.NPCData _data)
    {
        pos = _pos;
        cam = _cam;
        data = _data;

        parentRect = (Managers.UIM.SceneUI as UI_GameScene).WorldSpeechParent.GetComponent<RectTransform>();
        transform.SetParent(parentRect, false);

        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;

        StartSpeech();
    }

    void StartSpeech()
    {
        OpenAnim();
        if (data == null || data.SpeechList.Count == 0) return;

        int randnum = Random.Range(0, data.SpeechList.Count);
        text.text = data.SpeechList[randnum].Replace("\\n", "\n");
        
    }

    void OpenAnim()
    {
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void CloseAnim()
    {
        transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
    }

    private void LateUpdate()
    {
        if(!parentRect.gameObject.activeInHierarchy)
        {
            CloseAnim();
        }

        if (cam == null || parentRect == null) return;
         
        Vector3 viewportPos = cam.WorldToViewportPoint(pos + PosOffset);
        bool shouldEnable = viewportPos.z > 0;
        if (isTextEnabled != shouldEnable)
        {
            isTextEnabled = shouldEnable;
            text.gameObject.SetActive(shouldEnable); // text.enabled 보다 확실함
        }
        if (!shouldEnable) return;

        var rect = parentRect.rect;
        float x = (viewportPos.x - 0.5f) * rect.width;
        float y = (viewportPos.y - 0.5f) * rect.height;

        myRect.anchoredPosition = new Vector2(x, y);
    }

}
