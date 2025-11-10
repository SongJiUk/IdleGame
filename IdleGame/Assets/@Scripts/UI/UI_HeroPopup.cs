using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class UI_HeroPopup : UI_Popup
{
    Dictionary<string, Data.CreatureData> characterDic = new Dictionary<string, Data.CreatureData>();
   enum GameObjects
    {
        CharacterContentObject,
    }

    enum Texts
    {
        AttackText,
        PlayerCountText,
    }
    enum Buttons
    {
        HeroRecallButton,
        HeroEnforceButton
    }

    RectTransform rect;
    public override bool Init()
    {
        if (!base.Init()) return false;
        GameObjectsType = typeof(GameObjects);
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);

        BindObject(GameObjectsType);
        BindText(TextsType);
        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.HeroRecallButton).gameObject.BindEvent(OnClickHeroRecallButton);
        GetButton(ButtonsType, (int)Buttons.HeroEnforceButton).gameObject.BindEvent(OnClickHeroEnforceButton);

        rect = GetObject(GameObjectsType, (int)GameObjects.CharacterContentObject).GetComponent<RectTransform>();
        return true;
    }

    public void SetInfo()
    {
        for(int i =1; i<=7; i++)
        {
            Managers.DataM.CreatureDataDic.TryGetValue(i, out var data);
            characterDic.Add(data.Name, data);
        }

        var sort_dic = characterDic.OrderBy(x => x.Value.CharacterGrade);

        foreach(var data in sort_dic)
        {
            var go = Managers.ResourceM.Instantiate("UI_CharacterIcon");
            go.transform.parent = rect.transform;
            go.transform.localScale = Vector3.one;
            var icon = go.GetComponent<UI_CharacterIcon>();
            icon.Init();
            icon.SetInfo(data.Value);
        }

        //LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    void OnClickHeroRecallButton()
    {

    }

    void OnClickHeroEnforceButton()
    {

    }
}
