using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class UI_TitleScene : UI_Scene
{
    public override Transform WorldCoinParent
    {
        get { return null; }
    }

    public override Transform WorldFontParent
    {
        get { return null; }
    }
    public override Transform WorldJewelParent
    {
        get { return null; }
    }

    public override Transform WorldItemParent
    {
        get { return null; }
    }
    public enum Buttons
    {
        StartButton,
    }

    public enum Sliders
    {
        LoadingSlider
    }

    public enum Texts
    {
        CountText,
        MaxCountText,

    }

    bool isLoadEnd = false;
    public override bool Init()
    {
        if (!base.Init()) return false;

        ButtonsType = typeof(Buttons);
        SlidersType = typeof(Sliders);
        TextsType = typeof(Texts);

        BindButton(ButtonsType);
        //BindSlider(SlidersType);
        //BindText(TextsType);

        GetButton(ButtonsType, (int)Buttons.StartButton).gameObject.BindEvent(async () =>
        {
            //TOOD: 씬 이동
            await Managers.SceneM.LoadSceneAsync(Define.SceneType.GameScene);
        });

        GetButton(ButtonsType, (int)Buttons.StartButton).gameObject.SetActive(false);


        SetInfo().Forget();
        return true;
    }


    async UniTask SetInfo()
    {
        try
        {
            await Managers.ResourceM.LoadGroupAsync<UnityEngine.Object>("PrevLoad", (key, count, max) =>
            {
                //GetSlider(SlidersType, (int)Sliders.LoadingSlider).value = (float)count / max;
                //GetText(TextsType, (int)Texts.CountText).text = $"{count} / {max}";

                if (count == max)
                {
                    isLoadEnd = true;
                    GetButton(ButtonsType, (int)Buttons.StartButton).gameObject.SetActive(true);
                    //GetText(typeof(Texts), (int)Texts.CountText).gameObject.SetActive(true);

                    //TODO : 초기화 작업
                    Managers.DataM.Init();
                }
            });
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }



}
