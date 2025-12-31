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
    Dictionary<string, CharacterHolder> characterDic = new Dictionary<string, CharacterHolder>();
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
        Circle1Button,
        Circle2Button,
        Circle3Button,
        Circle4Button,
        Circle5Button,
        Circle6Button,
        HeroGachaButton,
        HeroEnforceButton,
        CloseButton
    }
    UI_CharacterIcon clickCharacter;
    RectTransform rect;
    public System.Action OnValueChange;
    bool isRemoveCharacter = false;
    List<CharacterHolder> characterList = new List<CharacterHolder>();
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        Managers.StageM.playEvent -= OnPlay;
        Managers.StageM.playEvent += OnPlay;
        GameObjectsType = typeof(GameObjects);
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);

        BindObject(GameObjectsType);
        BindText(TextsType);
        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.HeroGachaButton).gameObject.BindEvent(OnClickHeroGachaButton);
        GetButton(ButtonsType, (int)Buttons.HeroEnforceButton).gameObject.BindEvent(OnClickHeroEnforceButton);
        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);


        foreach (Buttons buttonType in Enum.GetValues(typeof(Buttons)))
        {
            GetButton(ButtonsType, (int)buttonType).gameObject.BindEvent(() => OnClickCircleButton(buttonType));
        }

        rect = GetObject(GameObjectsType, (int)GameObjects.CharacterContentObject).GetComponent<RectTransform>();
        Managers.RenderM.renderCharacter.InitCharacter();

        return true;
    }

    //처음에만 사용할것인지? 아이템을 뽑거나 할때는 수정이 되어야함
    public override void SetInfo()
    {
        Managers.RenderM.renderCharacter.GetRenderCharacterParitcle(false);
        var datas = Managers.GameM.gameData.Characters_Data;
        characterDic.Clear();

        foreach (var data in datas)
        {
            characterDic.Add(data.Value.data.Name, data.Value);
        }

        //TODO : 정렬 방법 바꾸기
        var sortdic = characterDic.OrderBy(x => x.Value.data.CharacterGrade);

        foreach (var data in sortdic)
        {
            if (data.Value.holder.Count == 0) continue;

            var go = Managers.UIM.MakeSubItem<UI_CharacterIcon>();
            go.transform.SetParent(rect.transform, false);
            go.transform.localScale = Vector3.one;

            go.Init().Forget();
            go.SetInfo(data.Value.data, this);
            characters.Add(go);
        }
    }


    void OnClickHeroGachaButton()
    {
        //TODO : 해당 팝업 끄고, shopPopup으로 이동
        Managers.UIM.ClosePopup(this).Forget();
        Managers.UIM.ShowPopup<UI_ShopPopup>().Forget();
    }

    async void OnClickHeroEnforceButton()
    {
        if (CheckUpgradeCharacter())
        {
            var popup = await Managers.UIM.ShowPopup<UI_UpgradePopup>();
            popup.SetInfo(characterList);
        }
        else
        {
            Managers.UIM.ShowToast("강화가 가능한 캐릭터가 없습니다.");
        }

    }

    bool CheckUpgradeCharacter()
    {
        characterList.Clear();
        foreach (var character in Managers.GameM.gameData.Characters_Data)
        {
            var data = character.Value;
            if (data.holder.Level == 0) continue;
            int needCount = data.holder.Level * 5;
            if (needCount <= data.holder.Count)
            {
                data.holder.Count -= needCount;
                data.holder.Level++;
                OnValueChange?.Invoke();
                characterList.Add(data);
            }
        }

        if (characterList.Count == 0) return false;
        else return true;
    }

    void OnClickCloseButton()
    {
        TriggerClose(this, true);

        characterDic.Clear();
        for (int i = 0; i < characters.Count; i++)
        {
            Managers.ResourceM.Destroy(characters[i].gameObject);
        }
        characters.Clear();

    }

    void OnClickCircleButton(Buttons _clickButton)
    {
        //일단 선택된 캐릭터가 없으면 return
        if (clickCharacter == null) return;
        if (_clickButton > Buttons.Circle6Button) return;

        InitCharacter(_clickButton);
    }

    void InitCharacter(Buttons _clickButton)
    {

        if (Managers.RenderM.renderCharacter.isCheckCharacter((int)_clickButton))
        {
            //TODO : 여기는 해당 버튼에 캐릭터가 있을때
            Managers.CharacterM.SetCharacter((int)_clickButton, clickCharacter.DATA.Name);
            Managers.RenderM.renderCharacter.GetRenderCharacterParitcle(false);
            SetClick(null);
            (Managers.UIM.SceneUI as UI_GameScene).CheckCharactersState();

            Managers.RenderM.renderCharacter.ChangeCharacter();
            OnValueChange?.Invoke();
            clickCharacter = null;
        }
        else
        {
            //TODO : 전에클릭되어있던 clickCharacter정보 가져와서 거기 초기화 시켜줘야함

            Managers.CharacterM.SetCharacter((int)_clickButton, clickCharacter.DATA.Name);
            //해당 지역 이펙트
            Managers.RenderM.renderCharacter.GetRenderCharacterParitcle(false);
            //해당 아이콘 잠금 처리?
            SetClick(null);
            //TODO : 아직 게임 진행중일때, 대기중 사진 띄우기.
            (Managers.UIM.SceneUI as UI_GameScene).CheckCharactersState();


            //여기서 해당 지역에 있던 오브젝트를 변경해줘야할듯
            Managers.RenderM.renderCharacter.InitCharacter();
            OnValueChange?.Invoke();
            clickCharacter = null;
        }
    }


    void OnlyRemoveCharacter()
    {
        Managers.CharacterM.GetCharacter(clickCharacter.DATA.Name);
        SetClick(null);
        isRemoveCharacter = true;

        Managers.RenderM.renderCharacter.RemoveCharacter();
        OnValueChange?.Invoke();
        clickCharacter = null;
    }

    public void SetClick(UI_CharacterIcon _clickCharacter, bool _isMinusClick = false)
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
            if (_isMinusClick)
            {
                OnlyRemoveCharacter();
            }
            else
            {
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

    public void OnPlay()
    {
        (Managers.UIM.SceneUI as UI_GameScene).CheckCharactersState();
        isRemoveCharacter = false;
    }

}
