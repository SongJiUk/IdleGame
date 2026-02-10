using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class UI_AchievementPopup : UI_Popup
{
    #region enum
    enum GameObjects
    {
        Content,
        ScrollView,
    }

    enum Texts
    {
        AchievementAbilityText
    }

    enum Buttons
    {
        CloseButton,

    }
    #endregion
    Transform parent;
    List<UI_AchievementItem> itemPool = new List<UI_AchievementItem>();
    ScrollRect scrollRect;
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        GameObjectsType = typeof(GameObjects);
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);

        BindObject(GameObjectsType);
        BindText(TextsType);
        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        scrollRect = GetObject(GameObjectsType, (int)GameObjects.ScrollView).GetComponent<ScrollRect>();
        parent = GetObject(GameObjectsType, (int)GameObjects.Content).transform;
        return true;
    }


    public override void SetInfo()
    {
        int index = 0;
        foreach (var data in Managers.DataM.AchievementDataDic)
        {
            UI_AchievementItem item;
            if (index < itemPool.Count)
            {
                item = itemPool[index];
                item.gameObject.SetActive(true);
            }
            else
            {
                item = Managers.UIM.MakeSubItem<UI_AchievementItem>(parent);
                item.Init().Forget();
                itemPool.Add(item);
            }

            item.SetInfo(data.Value);
            item.OnCollected = RefreshUI;
            index++;
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
        RefreshUI();
    }
    void RefreshUI()
    {        
        for (int i = 0; i < itemPool.Count; i++)
        {
            if (Managers.QuestM.AchievementDic[itemPool[i].data.AchievementID])
            {
                itemPool[i].transform.SetAsLastSibling();
            }
        }

        GetText(TextsType, (int)Texts.AchievementAbilityText).text = string.Format("{0} / {1}\n{2} / {3}\n{4} / {5}\n{6} / {7}",
        LocalTemp(Define.Status_Holder.Damage, Managers.QuestM.Achievement_Status_Data.damage),
        LocalTemp(Define.Status_Holder.HP, Managers.QuestM.Achievement_Status_Data.hp),
        LocalTemp(Define.Status_Holder.Money, Managers.QuestM.Achievement_Status_Data.money),
        LocalTemp(Define.Status_Holder.Item, Managers.QuestM.Achievement_Status_Data.item),
        LocalTemp(Define.Status_Holder.Skill, Managers.QuestM.Achievement_Status_Data.skill),
        LocalTemp(Define.Status_Holder.AttackSpeed, Managers.QuestM.Achievement_Status_Data.attackSpeed),
        LocalTemp(Define.Status_Holder.CriticalP, Managers.QuestM.Achievement_Status_Data.criticalP),
        LocalTemp(Define.Status_Holder.CriticalD, Managers.QuestM.Achievement_Status_Data.criticalD)
        );
    }
    private string PlusOrMinus(double _value)
    {
        var temp = (int)Mathf.Sign((float)_value) == 1 ? " + " : " - ";
        return temp;
    }

    private string LocalTemp(Define.Status_Holder _holder, double _value)
    {
        string color;
        if(_value != 0 )
        { 
        color = Utils.StringToColorGrade(Define.Grade.UnCommon);
        }
        else
        color = Utils.StringToColorGrade(Define.Grade.Common);
        

        string temp = color + Managers.LocalM.localData[_holder.ToString()].GetData() + PlusOrMinus(_value) + _value.ToString() + "%" +"</color>";

        return temp;
    }

    void OnClickCloseButton()
    {
        Managers.UIM.ClosePopup(this).Forget();

    }
}
