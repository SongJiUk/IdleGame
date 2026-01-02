using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_SavingMode : UI_Popup, ITickable
{
    #region Enum
    enum GameObjects
    {
        Content
    }
    enum Images
    {
        BackGround,
        CharacterBG,
        Battery_Image_Fill,

    }

    enum Texts
    {
        Battery_Text,
        TImeText,
        RoundText,
        RoundSituationText,
        UnLockText,
    }
    #endregion

    List<UI_Item> itemPool = new List<UI_Item>();
    Dictionary<string, ItemHolder> SavingModeItem = new Dictionary<string, ItemHolder>();
    Transform parent = null;
    Vector2 dragStartPos;
    Vector2 dragEndPos;
    float dragDist;
    Camera cam;

    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        GameObjectsType = typeof(GameObjects);
        ImagesType = typeof(Images);
        TextsType = typeof(Texts);

        BindObject(GameObjectsType);
        BindImage(ImagesType);
        BindText(TextsType);

        parent = GetObject(GameObjectsType, (int)GameObjects.Content).transform;
        return true;
    }
    public override void SetInfo()
    {
        cam = Camera.main;
        cam.enabled = false;
        Managers.UpdateM.Register(this);
    }


    public void Tick(float _deltaTime)
    {
        //핸드폰에서만 됌.
        GetText(TextsType, (int)Texts.Battery_Text).text = (SystemInfo.batteryLevel * 100f).ToString();
        GetImage(ImagesType, (int)Images.Battery_Image_Fill).fillAmount = SystemInfo.batteryLevel;
        int battery = (int)(SystemInfo.batteryLevel * 100f);
        if (battery > 20) GetImage(ImagesType, (int)Images.Battery_Image_Fill).color = Color.green;
        else GetImage(ImagesType, (int)Images.Battery_Image_Fill).color = Color.red;


        GetText(TextsType, (int)Texts.TImeText).text = System.DateTime.Now.ToString("HH:mm");

        int stageValue = Managers.GameM.Stage;
        int stageForward = (stageValue / 20) + 1;
        int stageBack = stageValue % 20;
        GetText(TextsType, (int)Texts.RoundText).text = stageForward.ToString() + " - " + stageBack.ToString();

        GetText(TextsType, (int)Texts.RoundSituationText).text = Managers.StageM.isDead ? "반복중..." : "진행중...";
        GetText(TextsType, (int)Texts.RoundSituationText).color = Managers.StageM.isDead ? Color.yellow : Color.blue;

        gameObject.BindEvent(_dragAction: OnBeginDrag, _type: Define.UIEvent.BeginDrag);
        gameObject.BindEvent(_dragAction: OnDrag, _type: Define.UIEvent.Drag);
        gameObject.BindEvent(_dragAction: OnEndDrag, _type: Define.UIEvent.EndDrag);

    }
    public void GetItem(Data.ItemData _itemData)
    {

        if (SavingModeItem.ContainsKey(_itemData.Name))
        {
            SavingModeItem[_itemData.Name].holder.Count++;
            RefreshItem(_itemData);
            return;
        }
        ItemHolder item = new ItemHolder { data = _itemData, holder = new Holder() };
        item.holder.Count = 1;

        SavingModeItem.Add(_itemData.Name, item);

        RefreshItem(_itemData);

    }

    public void RefreshItem(Data.ItemData _itemData)
    {
        int needCount = SavingModeItem.Count;

        while (itemPool.Count < needCount)
        {
            UI_Item i = Managers.UIM.MakeSubItem<UI_Item>(parent);
            itemPool.Add(i);
        }

        foreach (var slot in itemPool)
            slot.gameObject.SetActive(false);

        int index = 0;
        var sort_items = SavingModeItem.OrderByDescending(x => x.Value.data.ItemGrade);

        foreach (var item in sort_items)
        {
            UI_Item slot = itemPool[index++];
            slot.gameObject.SetActive(true);

            slot.Init().Forget();
            slot.SetInfo(item.Value);
        }
    }

    public override void OnBeginDrag(BaseEventData _eventData)
    {
        PointerEventData pointerEventData = _eventData as PointerEventData;
        if (pointerEventData != null)
        {
            dragStartPos = pointerEventData.position;
        }
    }
    public override void OnDrag(BaseEventData _eventData)
    {
        PointerEventData pointerEventData = _eventData as PointerEventData;
        if (pointerEventData != null)
        {
            dragEndPos = pointerEventData.position;

            dragDist = Vector2.Distance(dragEndPos, dragStartPos);

            float threshold = Screen.width / 2;
            float dragProgress = Mathf.Clamp01(dragDist / threshold);
            float minAlpha = 0.3f;
            float maxAlpha = 1f;

            float targetAlpha = Mathf.Lerp(maxAlpha, minAlpha, dragProgress);

            GetImage(ImagesType, (int)Images.BackGround).color = new Color(0f, 0f, 0f, targetAlpha);
            GetImage(ImagesType, (int)Images.CharacterBG).color = new Color(1f, 1f, 1f, targetAlpha);

        }
    }

    public override void OnEndDrag(BaseEventData _eventData)
    {
        if (dragDist >= Screen.width / 2)
        {
            PopupClose();
        }

        dragStartPos = Vector2.zero;
        dragEndPos = Vector2.zero;
        dragDist = 0;

        GetImage(ImagesType, (int)Images.BackGround).color = new Color(0f, 0f, 0f, 1f);
        GetImage(ImagesType, (int)Images.CharacterBG).color = new Color(1f, 1f, 1f, 1f);
    }
    public void PopupClose()
    {
        //TODO : 꺼지면, 초기화 시켜야됌 아이템 얻은 창
        SavingModeItem.Clear();

        foreach (var slot in itemPool)
            slot.gameObject.SetActive(false);
        itemPool.Clear();

        Managers.UpdateM.UnRegister(this);
        cam.enabled = true;
        (Managers.UIM.SceneUI as UI_GameScene).isSavingMode = false;

        Managers.UIM.ClosePopup(this).Forget();
    }
}

