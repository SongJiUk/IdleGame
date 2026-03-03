using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterIcon : UI_Base
{
    #region Enum
    enum GameObjects
    {
        CharacterLockObject,
        CharacterUseObject
    }
    enum Buttons
    {
        UI_CharacterIcon,
        CharacterUseButton,

    }
    enum Images
    {
        UI_CharacterIcon,
        CharacterImage,
        CountFillImage,
        PlusImage,
        MinusImage

    }
    enum Texts
    {
        CharacterCountText,
        CharacterLevelText
    }
    #endregion

    Data.CreatureData data;
    public Data.CreatureData DATA
    {
        get { return data; }
    }
    UI_HeroPopup parent;
    bool isUseCharacter = false;
    public int slotIndex;

    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        GameObjectsType = typeof(GameObjects);
        ImagesType = typeof(Images);
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);

        BindObject(GameObjectsType);
        BindImage(ImagesType);
        BindText(TextsType);
        BindButton(ButtonsType);

        //TODO: 캐릭터 정보창
        GetButton(ButtonsType, (int)Buttons.UI_CharacterIcon).gameObject.BindEvent(OnClickCharacterInfo);
        GetButton(ButtonsType, (int)Buttons.CharacterUseButton).gameObject.BindEvent(OnClickCharacter);


        return true;
    }

    public void SetInfo(Data.CreatureData _data, UI_HeroPopup _parent)
    {
        data = _data;
        parent = _parent;
        parent.OnValueChange -= RefreshUI;
        parent.OnValueChange += RefreshUI;

        GetImage(ImagesType, (int)Images.UI_CharacterIcon).sprite = Managers.ResourceM.GetAtlas(_data.CharacterGrade.ToString());
        GetImage(ImagesType, (int)Images.CharacterImage).sprite = Managers.ResourceM.GetAtlas(_data.PrefabName);
        GetImage(ImagesType, (int)Images.CharacterImage).SetNativeSize();

        GetImage(ImagesType, (int)Images.PlusImage).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.MinusImage).gameObject.SetActive(false);

        GetObject(GameObjectsType, (int)GameObjects.CharacterUseObject).SetActive(false);

        if (data.DataID == 1)
        {
            GetButton(ButtonsType, (int)Buttons.CharacterUseButton).gameObject.SetActive(false);
        }
        else GetButton(ButtonsType, (int)Buttons.CharacterUseButton).gameObject.SetActive(true);
        RefreshUI();
    }
    public void RefreshUI()
    {
        int levelCount = Managers.GameM.gameData.Character_Holder[data.Name].Level * 5;
        GetImage(ImagesType, (int)Images.CountFillImage).fillAmount = (float)Managers.GameM.gameData.Character_Holder[data.Name].Count / (float)levelCount;
        GetText(TextsType, (int)Texts.CharacterCountText).text = Managers.GameM.gameData.Character_Holder[data.Name].Count.ToString() + " / " + levelCount.ToString();
        GetText(TextsType, (int)Texts.CharacterLevelText).text = "Lv. " + Managers.GameM.gameData.Character_Holder[data.Name].Level.ToString();

        this.GetComponent<Outline>().enabled = false;
        GetObject(GameObjectsType, (int)GameObjects.CharacterLockObject).SetActive(false);

        CheckUseCharacter();
    }

    public void CheckUseCharacter()
    {
        isUseCharacter = false;
        for (int i = 0; i < Managers.CharacterM.Characters.Length; i++)
        {
            if (Managers.CharacterM.Characters[i] == null) continue;

            if (Managers.CharacterM.Characters[i].data == data)
            {
                isUseCharacter = true;
            }
        }

        GetImage(ImagesType, (int)Images.MinusImage).gameObject.SetActive(isUseCharacter);
        GetImage(ImagesType, (int)Images.PlusImage).gameObject.SetActive(!isUseCharacter);

        GetObject(GameObjectsType, (int)GameObjects.CharacterUseObject).SetActive(isUseCharacter);
    }

    public async void OnClickCharacterInfo()
    {
        var popup = await Managers.UIM.ShowPopup<UI_CharacterInfoPopup>();
        popup.SetInfo(data);
        popup.OnChangeCharacterInfo -= CheckCharacterInfo;
        popup.OnChangeCharacterInfo += CheckCharacterInfo;
    }

    public void CheckCharacterInfo()
    {
        RefreshUI();
    }

    public void OnClickCharacter()
    {
        if (isUseCharacter)
        {
            parent.SetClick(this, true);
            //Managers.RenderM.renderCharacter.GetRenderCharacterParticle(false);
        }
        else
        {
            parent.SetClick(this);
            //Managers.RenderM.renderCharacter.GetRenderCharacterParticle(false);
        }

    }

    public void SetLockImage(bool _isLock)
    {
        GetObject(GameObjectsType, (int)GameObjects.CharacterLockObject).SetActive(_isLock);
    }

}
