using UnityEngine;
using DG.Tweening;
using TMPro;

public class SpeechBubble : UIDirecting
{
    [SerializeField] TextMeshProUGUI text;

    private readonly Vector3 PosOffset = new Vector3(-1.1f, 1f, 0);

    Camera cam;
    RectTransform myRect;
    RectTransform parentRect;
    Transform target;

    private bool isTextEnabled = true;

    public override bool Init()
    {
        if (!base.Init()) return false;
        myRect = GetComponent<RectTransform>();
        return true;
    }

    public void Init(Transform _target, Camera _cam, Data.NPCData _data)
    {
        target = _target;
        cam = _cam;

        parentRect = (Managers.UIM.SceneUI as UI_GameScene).WorldSpeechParent.GetComponent<RectTransform>();
        transform.SetParent(parentRect, false);

        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;

        if (_data != null && _data.SpeechList != null && _data.SpeechList.Count > 0)
        {
            int rand = Random.Range(0, _data.SpeechList.Count);
            text.text = _data.SpeechList[rand].Replace("\\n", "\n");
        }

        OpenAnim();
    }

    void OpenAnim()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void CloseAnim()
    {
        transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
    }

    private void LateUpdate()
    {
        if (cam == null || parentRect == null || target == null) return;

        if (!parentRect.gameObject.activeInHierarchy)
        {
            CloseAnim();
            return;
        }

        // 1. 월드 좌표 → 스크린 좌표
        Vector3 worldPos = target.position + PosOffset;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        // 카메라 뒤면 숨김
        bool shouldEnable = screenPos.z > 0f;
        if (isTextEnabled != shouldEnable)
        {
            isTextEnabled = shouldEnable;
            gameObject.SetActive(shouldEnable);
        }
        if (!shouldEnable) return;

        // 2. 스크린 좌표 → 부모 RectTransform 로컬 좌표
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPos,
            null, // Overlay Canvas면 null
            out localPoint))
        {
            myRect.anchoredPosition = localPoint;
        }
    }
}