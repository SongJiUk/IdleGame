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
        RawImage,
        CircleButtonsObject,
    }

    enum Texts
    {
        AttackText,
        PlayerCountText,
    }
    enum Buttons
    {
        CloseButton,
        Circle1Button,
        Circle2Button,
        Circle3Button,
        Circle4Button,
        Circle5Button,
        Circle6Button,
        HeroGachaButton,
        HeroEnforceButton,
        
    }
    UI_CharacterIcon clickCharacter;
    RectTransform rect;
    public System.Action OnValueChange;
    bool isRemoveCharacter = false;
    List<CharacterHolder> characterList = new List<CharacterHolder>();
    RawImage rawImage;
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
        rawImage = GetObject(GameObjectsType, (int)GameObjects.RawImage).GetComponent<RawImage>();
        Managers.RenderM.renderCharacter.InitCharacter();

        return true;
    }

    //처음에만 사용할것인지? 아이템을 뽑거나 할때는 수정이 되어야함
    public override void SetInfo()
    {
        GetText(TextsType, (int)Texts.AttackText).text = Utils.ToCurrencyString(Managers.PlayerM.AverageCombatPower());

        GetObject(GameObjectsType, (int)GameObjects.CircleButtonsObject).SetActive(false);
        Managers.RenderM.Show(RenderType.Hero, rawImage);

        var datas = Managers.GameM.gameData.Characters_Data;


        if (characters.Count != 0) ClearIcons();


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

        //Managers.RenderM.renderCharacter.GetRenderCharacterParticle(true);
    }   


    async void OnClickHeroGachaButton()
    {
        if (!Managers.UIM.ClickLock(0.5f)) return;
        var gameScene = Managers.UIM.SceneUI as UI_GameScene;
        if(gameScene != null)
        {
            await Managers.UIM.ClosePopup(this);
            gameScene.OpenShopFormOtherUI().Forget();
        }

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

    async void OnClickCloseButton()
    {

        await TriggerClose(this, true);

        ClearIcons();
    }

    private void OnDisable()
    {
        Managers.RenderM.Hide();
    }
    void ClearIcons()
    {
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
        if(_clickButton < Buttons.Circle1Button) return;
        InitCharacter(_clickButton);
    }

    void InitCharacter(Buttons _clickButton)
    {

        if(!Managers.StageM.CanChangeToReady())
        {
            return;
        }

        clickCharacter.slotIndex = (int)_clickButton;
        if (clickCharacter != null && clickCharacter.DATA != null)
        {
            Managers.GameM.gameData.TeamPlacementID[clickCharacter.slotIndex] = clickCharacter.DATA.DataID;
            Managers.FirebaseM.WriteData().Forget();
        }

        if (Managers.RenderM.renderCharacter.isCheckCharacter((int)_clickButton))
        {
            Managers.CharacterM.SetCharacter((int)_clickButton, clickCharacter.DATA.Name);
            SetClick(null);
            (Managers.UIM.SceneUI as UI_GameScene).CheckCharactersState();

            Managers.RenderM.renderCharacter.ChangeCharacter();
            OnValueChange?.Invoke();
            clickCharacter = null;
            //Managers.RenderM.renderCharacter.GetRenderCharacterParticle(true);
            Managers.StageM.StateChange(Define.StageState.Ready);
            
        }
        else
        {
            Managers.CharacterM.SetCharacter((int)_clickButton, clickCharacter.DATA.Name);
            SetClick(null);
            //Managers.RenderM.renderCharacter.GetRenderCharacterParticle(true);
            (Managers.UIM.SceneUI as UI_GameScene).CheckCharactersState();


            //여기서 해당 지역에 있던 오브젝트를 변경해줘야할듯
            Managers.RenderM.renderCharacter.InitCharacter();
            OnValueChange?.Invoke();
            clickCharacter = null;
            Managers.StageM.StateChange(Define.StageState.Ready);
        }
        GetObject(GameObjectsType, (int)GameObjects.CircleButtonsObject).SetActive(false);
    }


    void OnlyRemoveCharacter(int _slotIndex)
    {
        if (!Managers.StageM.CanChangeToReady())
        {
            return;
        }

        Managers.GameM.gameData.TeamPlacementID[_slotIndex] = 0;
        Managers.FirebaseM.WriteData().Forget();

        Managers.CharacterM.GetCharacter(clickCharacter.DATA.Name);
        SetClick(null);
        isRemoveCharacter = true;



        Managers.RenderM.renderCharacter.RemoveCharacter();
        OnValueChange?.Invoke();
        clickCharacter = null;

        Managers.StageM.StateChange(Define.StageState.Ready); 
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
                OnlyRemoveCharacter(clickCharacter.slotIndex);
            }
            else
            {
                GetObject(GameObjectsType, (int)GameObjects.CircleButtonsObject).SetActive(true);
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

    public void OnPlay(Define.StageState _state)
    {
        (Managers.UIM.SceneUI as UI_GameScene).CheckCharactersState();
        isRemoveCharacter = false;
    }
}
