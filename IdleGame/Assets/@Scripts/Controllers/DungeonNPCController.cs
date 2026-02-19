using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class DungeonNPCController : BaseController
{
    [SerializeField] int NPCDataID;
    [SerializeField] Camera cam;
    bool isDugeonPopupOn = false;
    SpeechBubble currentBubble;

    public void SetInfo()
    {
        Managers.UIM.OnDungeonPopupState -= OnDungeonPopup;
        Managers.UIM.OnDungeonPopupState += OnDungeonPopup;
    }

    private void OnDisable()
    {
        Managers.UIM.OnDungeonPopupState -= OnDungeonPopup;
    }

    void OnDungeonPopup(bool _isOpen)
    {
        isDugeonPopupOn = _isOpen;
        if (_isOpen)
        {
            StartSpeech().Forget();
        }
        else
        {
            if (currentBubble != null)
            {
                Managers.ObjectM.DeSpawn(currentBubble);
                currentBubble = null;
            }
        }
    }

    async UniTaskVoid StartSpeech()
    {
        while (isDugeonPopupOn)
        {
            await UniTask.WaitForSeconds(Random.Range(3.0f, 7.0f));
            if (!isDugeonPopupOn) break;

            if (currentBubble != null) Managers.ObjectM.DeSpawn(currentBubble);
            currentBubble = Managers.ObjectM.Spawn<SpeechBubble>(transform.position, Utils.SpeechBubbleDataID);

            Managers.DataM.NPCDataDic.TryGetValue(NPCDataID, out var data);
            if (currentBubble != null)
            {
                currentBubble.Init(transform, cam, data);
            }
            await UniTask.WaitForSeconds(3.0f);

            if (currentBubble != null)
            {
                Managers.ObjectM.DeSpawn(currentBubble);
                currentBubble = null;
            }
        }
    }

    private void OnDestroy()
    {
        isDugeonPopupOn = false;
        if (Managers.UIM != null) Managers.UIM.OnDungeonPopupState -= OnDungeonPopup;
    }
}
