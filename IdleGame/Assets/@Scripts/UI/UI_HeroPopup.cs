using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class UI_HeroPopup : UI_Popup
{
    //TODO : 렌더텍스쳐 해상도 값도 수정해줘야함.
    public List<UI_CharacterIcon> characters = new List<UI_CharacterIcon>();
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
        Circle0Button,
        Circle1Button,
        Circle2Button,
        Circle3Button,
        Circle4Button,
        Circle5Button,
        Circle6Button,
        HeroRecallButton,
        HeroEnforceButton,
        CloseButton
    }
    UI_CharacterIcon clickCharacter;
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


        foreach (Buttons buttonType in Enum.GetValues(typeof(Buttons)))
        {
            //TODO : 이거 CIrcle0 ~ 6까지만 되어야됌
            GetButton(ButtonsType, (int)buttonType).gameObject.BindEvent(() => OnClickCircleButton(buttonType));
        }

        rect = GetObject(GameObjectsType, (int)GameObjects.CharacterContentObject).GetComponent<RectTransform>();
        Managers.RenderM.renderCharacter.InitCharacter();
        SetInfo();
        return true;
    }

    //처음에만 사용할것인지? 아이템을 뽑거나 할때는 수정이 되어야함
    public void SetInfo()
    {
        //TODO : 이거 꺼졌다 켜질떄마다 계속 생성되게 하면 안됨 고쳐야됌
        //TODO : 그리고 가지고있는 데이터에 맞게 호출해야된다.
        var datas = Managers.GameM.gameData.DataCharacter;

        foreach (var data in datas)
        {
            characterDic.Add(data.Value.data.Name, data.Value.data);
        }

        var sortdic = characterDic.OrderBy(x => x.Value.CharacterGrade);

        foreach (var data in sortdic)
        {
            //TODO : 이것도 수정(오브젝트 매니저 사용하는걸로)
            var go = Managers.ResourceM.Instantiate("UI_CharacterIcon");
            go.transform.parent = rect.transform;
            go.transform.localScale = Vector3.one;
            var icon = go.GetComponent<UI_CharacterIcon>();
            icon.Init().Forget();
            icon.SetInfo(data.Value, this);
            characters.Add(icon);
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

    void OnClickCircleButton(Buttons _clickButton)
    {
        //일단 선택된 캐릭터가 없으면 return
        if (clickCharacter == null) return;
        if (_clickButton > Buttons.Circle6Button) return;
        if (Managers.RenderM.renderCharacter.isCheckCharacter((int)_clickButton)) return;


        //TODO : 이렇게 해서 해당 지역 체크 후 넣어주기
        Managers.CharacterM.GetCharacter((int)_clickButton, clickCharacter.DATA.Name);
        Managers.RenderM.renderCharacter.GetRenderCharacterParitcle(false);
        SetClick(null);
        //TODO : 아직 게임 진행중일때, 대기중 사진 띄우기.
        (Managers.UIM.SceneUI as UI_GameScene).CheckCharactersState();

        Managers.RenderM.renderCharacter.InitCharacter();
        clickCharacter.CheckUseCharacter();

        clickCharacter = null;
    }



    public void SetClick(UI_CharacterIcon _clickCharacter)
    {

        if (_clickCharacter == null)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                characters[i].SetLockImage(false);
                characters[i].GetComponent<Outline>().enabled = false;
            }
        }
        else
        {
            clickCharacter = _clickCharacter;
            for (int i = 0; i < characters.Count; i++)
            {
                characters[i].SetLockImage(true);
                characters[i].GetComponent<Outline>().enabled = false;
            }

            clickCharacter.SetLockImage(false);
            clickCharacter.GetComponent<Outline>().enabled = true;
        }
    }


}
