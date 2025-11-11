using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

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
        HeroEnforceButton,
        CloseButton
    }

    RectTransform rect;
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        GameObjectsType = typeof(GameObjects);
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);

        BindObject(GameObjectsType);
        BindText(TextsType);
        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.HeroRecallButton).gameObject.BindEvent(OnClickHeroRecallButton);
        GetButton(ButtonsType, (int)Buttons.HeroEnforceButton).gameObject.BindEvent(OnClickHeroEnforceButton);
        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        rect = GetObject(GameObjectsType, (int)GameObjects.CharacterContentObject).GetComponent<RectTransform>();
        SetInfo();
        return true;
    }

    //처음에만 사용할것인지? 아이템을 뽑거나 할때는 수정이 되어야함
    public void SetInfo()
    {
        //TODO : 이거 꺼졌다 켜질떄마다 계속 생성되게 하면 안됨 고쳐야됌
        //TODO : 그리고 가지고있는 데이터에 맞게 호출해야된다.
        for (int i = 1; i <= 7; i++)
        {
            Managers.DataM.CreatureDataDic.TryGetValue(i, out var data);
            characterDic.Add(data.Name, data);
        }

        var sort_dic = characterDic.OrderBy(x => x.Value.CharacterGrade);

        foreach (var data in sort_dic)
        {
            //TODO : 이것도 수정(오브젝트 매니저 사용하는걸로)
            var go = Managers.ResourceM.Instantiate("UI_CharacterIcon");
            go.transform.parent = rect.transform;
            go.transform.localScale = Vector3.one;
            var icon = go.GetComponent<UI_CharacterIcon>();
            icon.Init().Forget();
            icon.SetInfo(data.Value);
        }
    }

    void OnClickHeroRecallButton()
    {

    }

    void OnClickHeroEnforceButton()
    {

    }

    void OnClickCloseButton()
    {
        TriggerClose(this);
    }
}
