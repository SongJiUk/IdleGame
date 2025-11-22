using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using TMPro;

public class UI_GameScene : UI_Scene, ITickable
{
    //TODO : 이거 UI다 나누는게 편할거같긴함.(UI_TOP, Bottom으로 크개 두개로 나눠서 하던가, 아니면 그냥 연관있는것끼리 모아서 하던가)
    #region Enum
    enum GameObjects
    {
        LayersObject,
        JewelObject,
        CoinObject,
        StageMonsterCountObject,
        BossBoardObject,
        DeadFrameHandObject,
        ItemPopupObject,
        ItemTextPopupObject,

    }
    enum Buttons
    {
        QuestButton,
        StatButton,
        HeroButton,
        RelicsButton,
        DungeonButton,
        EnforceButton,
        ShopButton,
        MainSkillButton,
        Character1_Button,
        Character2_Button,
        Character3_Button,
        Character4_Button,
        Character5_Button,
        LevelUpButton,
        DeadFrameButton,
    }

    enum Texts
    {
        JewelText,
        CoinText,
        StageText,
        CharacterLevelText,
        UserNameText,
        UserCombatPowerText,
        QuestTitleText,
        QuestDescriptionText,
        QueestTutorialText,
        RewardItemText,
        ExpText,
        AttackText,
        HpText,
        NeedLevelUpText,
        GetExpText,
        StageMonsterCountText,
        BossHPText,
        BossText,
        BossBoardStageText,
        ItemPopupText,
        ItemText1,
        ItemText2,
        ItemText3,
        ItemText4,
        ItemText5

    }

    enum Images
    {
        CharacterImage,
        RewardItemImage,
        TutorialHandImage,
        MainSkillButton,
        Character1_Lock,
        Character1_Plus,
        Character1_Icon,
        Character1_CoolTimeImage,
        Character2_Lock,
        Character2_Plus,
        Character2_Icon,
        Character2_CoolTimeImage,
        Character3_Lock,
        Character3_Plus,
        Character3_Icon,
        Character3_CoolTimeImage,
        Character4_Lock,
        Character4_Plus,
        Character4_Icon,
        Character4_CoolTimeImage,
        Character5_Lock,
        Character5_Plus,
        Character5_Icon,
        Character5_CoolTimeImage,
        Exp_FillImage,
        FadeImage,
        StageMonsterCountImage,
        BossHpImage,
        ItemPopupFrameImage,
        ItemPopupItemImage,

    }

    enum Sliders
    {

    }

    #endregion

    Button selectedButton = null;
    Button clickedButton = null;
    Button statBtn;
    Button heroBtn;
    Button relicsBtn;
    Button dungeonBtn;
    Button enforceBtn;
    Button shopBtn;
    Button levelUpBtn;
    Button deadFrameBtn;
    #region 코인, 쥬얼리, 아이템 애니메이션 관련
    //TODO : UI_Scene에서 관리하는거임
    public override Transform WorldCoinParent
    {
        get
        {
            return GetLayer((int)Define.UILayerIndex.Coin);
        }
    }
    public override Transform WorldJewelParent
    {
        get
        {
            return GetLayer((int)Define.UILayerIndex.Coin);
        }
    }

    public override Transform WorldFontParent
    {
        get
        {
            return GetLayer((int)Define.UILayerIndex.DamageFont);
        }
    }
    public override Transform WorldItemParent
    {
        get
        {
            return GetLayer((int)Define.UILayerIndex.ItemRect);
        }
    }


    #endregion

    #region 아이템 획득 애니메이션 관련
    RectTransform ItemPopupObjectRect;
    RectTransform ItemPopupFrameObjectRect;

    List<TextMeshProUGUI> itemTexts = new List<TextMeshProUGUI>();
    #endregion

    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        Managers.StageM.readyEvent += OnReady;
        Managers.StageM.playEvent += OnPlay;
        Managers.StageM.bossPlayEvent += OnBossPlay;
        Managers.StageM.clearEvent += OnClear;
        Managers.StageM.deadEvent += OnDead;

        Managers.GameM.OnGoodsChanged += RefreshGoods;

        Managers.UpdateM.Register(this);

        GameObjectsType = typeof(GameObjects);
        ButtonsType = typeof(Buttons);
        TextsType = typeof(Texts);
        ImagesType = typeof(Images);
        SlidersType = typeof(Sliders);

        BindObject(GameObjectsType);
        BindButton(ButtonsType);
        BindText(TextsType);
        BindImage(ImagesType);
        BindSlider(SlidersType);


        coinDirectingTr = GetObject(GameObjectsType, (int)GameObjects.CoinObject).GetComponent<RectTransform>();
        jewelDirectingTr = GetObject(GameObjectsType, (int)GameObjects.JewelObject).GetComponent<RectTransform>();
        ItemPopupObjectRect = GetObject(GameObjectsType, (int)GameObjects.ItemPopupObject).GetComponent<RectTransform>();
        ItemPopupFrameObjectRect = GetImage(ImagesType, (int)Images.ItemPopupFrameImage).GetComponent<RectTransform>();

        GameObject obj = GetObject(GameObjectsType, (int)GameObjects.ItemTextPopupObject);
        for (int i =0; i<obj.transform.childCount; i++)
        {
            itemTexts.Add(GetText(TextsType, (int)Texts.ItemText1 + i));
            itemTexts[i].gameObject.SetActive(false);
        }

        layers = GetObject(GameObjectsType, (int)GameObjects.LayersObject).GetComponent<Transform>();

        foreach (Buttons buttonType in Enum.GetValues(typeof(Buttons)))
        {
            GetButton(ButtonsType, (int)buttonType).gameObject.BindEvent(() => OnClickAnyButtons(buttonType).Forget());
        }

        GetButton(ButtonsType, (int)Buttons.LevelUpButton).gameObject.BindEvent(ClickDown, _type: Define.UIEvent.PointerDown);
        GetButton(ButtonsType, (int)Buttons.LevelUpButton).gameObject.BindEvent(ClickUp, _type: Define.UIEvent.PointerUp);

        statBtn = GetButton(ButtonsType, (int)Buttons.StatButton);
        heroBtn = GetButton(ButtonsType, (int)Buttons.HeroButton);
        relicsBtn = GetButton(ButtonsType, (int)Buttons.RelicsButton);
        dungeonBtn = GetButton(ButtonsType, (int)Buttons.DungeonButton);
        enforceBtn = GetButton(ButtonsType, (int)Buttons.EnforceButton);
        shopBtn = GetButton(ButtonsType, (int)Buttons.ShopButton);
        levelUpBtn = GetButton(ButtonsType, (int)Buttons.LevelUpButton);


        UpdateUIState();

        AllOff();
        StartSpawn();

        return true;
    }
    void OnDestroy()
    {
        Managers.StageM.readyEvent -= OnReady;
        Managers.StageM.playEvent -= OnPlay;
        Managers.StageM.bossPlayEvent -= OnBossPlay;
        Managers.StageM.clearEvent -= OnClear;
        Managers.StageM.deadEvent -= OnDead;

        Managers.GameM.OnGoodsChanged -= RefreshGoods;
    }

    void AllOff()
    {
        GetObject(GameObjectsType, (int)GameObjects.StageMonsterCountObject).SetActive(false);
        GetObject(GameObjectsType, (int)GameObjects.BossBoardObject).SetActive(false);
        GetButton(ButtonsType, (int)Buttons.DeadFrameButton).gameObject.SetActive(false);

        GetObject(GameObjectsType, (int)GameObjects.ItemPopupObject).SetActive(false);
    }
    void UpdateUIState()
    {
        
       
        //TODO : 여기서 이제 플레이어 상황 받아와서 bool값으로 처리해 주거나 더 좋은 방법생각해보자.
        GetImage(ImagesType, (int)Images.TutorialHandImage).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character1_Plus).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character2_Plus).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character3_Plus).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character4_Plus).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character5_Plus).gameObject.SetActive(false);

        GetImage(ImagesType, (int)Images.Character1_Icon).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character2_Icon).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character3_Icon).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character4_Icon).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character5_Icon).gameObject.SetActive(false);

        GetImage(ImagesType, (int)Images.Character1_CoolTimeImage).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character2_CoolTimeImage).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character3_CoolTimeImage).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character4_CoolTimeImage).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character5_CoolTimeImage).gameObject.SetActive(false);
    }
    //TODO :  코인, 쥬얼리 텍스트
    public void RefreshGoods()
    {
        GetText(TextsType, (int)Texts.CoinText).text = Utils.ToCurrencyString(Managers.GameM.Gold);
        GetText(TextsType, (int)Texts.JewelText).text = "0";
        CheckTexts();
    }
    async UniTaskVoid OnClickAnyButtons(Buttons _clickButtonType)
    {
        clickedButton = null;
        switch (_clickButtonType)
        {
            case Buttons.QuestButton:
                Debug.Log("Click Quest Button");
                break;
            case Buttons.StatButton:
                Debug.Log("Click Stat Button");
                clickedButton = statBtn;
                break;

            case Buttons.HeroButton:
                Debug.Log("Click Hero Button");
                clickedButton = heroBtn;
                ScaleUpSelectButton();

                UI_HeroPopup popup = await Managers.UIM.ShowPopup<UI_HeroPopup>(_isFade: true);
                popup.OnThisPopupClosed = ScaleDownSelectButton;
                // ui_HeroPopup.gameObject.SetActive(true);
                // ui_HeroPopup.SetInfo();
                // ui_HeroPopup.OnThisPopupClosed = ScaleDownSelectButton;

                break;

            case Buttons.RelicsButton:
                Debug.Log("Click Relics Button");
                clickedButton = relicsBtn;
                break;

            case Buttons.DungeonButton:
                Debug.Log("Click Dungeon Button");
                clickedButton = dungeonBtn;

                break;

            case Buttons.EnforceButton:
                Debug.Log("Click Enforce Button");
                clickedButton = enforceBtn;

                break;

            case Buttons.ShopButton:
                Debug.Log("Click Shop Button");
                clickedButton = shopBtn;
                break;

            case Buttons.LevelUpButton:
                OnClickLevelupButton();
                break;

            case Buttons.DeadFrameButton:
                Managers.StageM.isDead = false;
                Managers.StageM.StateChange(Define.StageState.Boss);
                break;
        }


    }

    private void ScaleUpSelectButton()
    {
        if (clickedButton == null) return;

        if (selectedButton != null && selectedButton != clickedButton)
        {
            selectedButton.transform.DOScale(Vector3.one, 0.2f);
        }

        clickedButton.transform.DOScale(Vector3.one * 1.2f, 0.2f);

        selectedButton = clickedButton;
    }

    private void ScaleDownSelectButton()
    {
        if (selectedButton != null)
        {
            selectedButton.transform.DOScale(Vector3.one, 0.2f);
            selectedButton = null;
        }
    }

    void CheckStageMonsterCount()
    {
        float value = (float)Managers.StageM.count / (float)Managers.StageM.maxCount;
        if (value >= 1.0f)
        {
            value = 1.0f;
            if (Managers.StageM.stageState != Define.StageState.Boss)
            {
                Managers.StageM.StateChange(Define.StageState.Boss);
            }

        }
        GetImage(ImagesType, (int)Images.StageMonsterCountImage).fillAmount = value;
        GetText(TextsType, (int)Texts.StageMonsterCountText).text = string.Format("{0:0.0}", value * 100.0f) + "%";
    }

    public void UpdateBossInfo(MonsterController _cc)
    {
        float value = (float)_cc.HP / (float)_cc.MaxHP;

        if (value <= 0.0f)
        {
            value = 0f;
        }
        GetImage(ImagesType, (int)Images.BossHpImage).fillAmount = (float)value;
        GetText(TextsType, (int)Texts.BossHPText).text = string.Format("{0:0.0}", value * 100.0f) + "%";
    }

    public void ResetStageBoard()
    {
        GetImage(ImagesType, (int)Images.StageMonsterCountImage).fillAmount = 0;
        GetText(TextsType, (int)Texts.StageMonsterCountText).text = "0%";
    }

    public void ResetBossBoard()
    {
        GetImage(ImagesType, (int)Images.BossHpImage).fillAmount = 1f;
        GetText(TextsType, (int)Texts.BossHPText).text = "100%";
    }

    #region Event
    public void OnReady()
    {
        AsyncFadeInOut(true).Forget();
    }
    public void OnPlay()
    {
        AllOff();
        if (Managers.StageM.isDead)
        {
            GetButton(ButtonsType, (int)Buttons.DeadFrameButton).gameObject.SetActive(true);
        }
        else
        {
            Managers.GameM.mPlayer.OnPlayerDataUpdate = CheckStageMonsterCount;
            GetObject(GameObjectsType, (int)GameObjects.StageMonsterCountObject).SetActive(true);
            ResetStageBoard();
        }
        
    }

    public void OnBossPlay()
    {
        AllOff();
        GetObject(GameObjectsType, (int)GameObjects.BossBoardObject).SetActive(true);
        ResetBossBoard();

    }

    public void OnClear()
    {
        AllOff();
    }
     
    public void OnDead()
    {
        AllOff();

    }

    #endregion
    
    #region 레벨업 버튼 애니메이션 관련
    void ClickDown()
    {
        ExpUPAnim();
        coroutine = StartCoroutine(coPush());

    }

    void ClickUp()
    {
        isLevelUpButtonPush = false;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        timer = 0.0f;
    }

    double needGold;
    public void OnClickLevelupButton()
    {
        needGold = Utils.CalculatedValue(Utils.Datas.levelData.Base_Gold, Managers.GameM.level, Utils.Datas.levelData.Player_Gold);
        if (Managers.GameM.Gold >= needGold)
        {
            Managers.GameM.Gold -= needGold;
            Managers.PlayerM.ExpUp();
            CheckTexts();
        }

        
    }

    public void CheckTexts()
    {
        needGold = Utils.CalculatedValue(Utils.Datas.levelData.Base_Gold, Managers.GameM.level, Utils.Datas.levelData.Player_Gold);

        GetImage(ImagesType, (int)Images.Exp_FillImage).fillAmount = Managers.PlayerM.ExpPercent();
        GetText(TextsType, (int)Texts.ExpText).text = string.Format("{0:0.00}", Managers.PlayerM.ExpPercent() * 100.0f) + "%";
        GetText(TextsType, (int)Texts.AttackText).text = $"+ {Utils.ToCurrencyString(Utils.Datas.levelData.Damage(Managers.GameM.mPlayer.BaseDamage))}";
        GetText(TextsType, (int)Texts.HpText).text = $"+ {Utils.ToCurrencyString(Utils.Datas.levelData.HP(Managers.GameM.mPlayer.BaseHp))}";
        GetText(TextsType, (int)Texts.NeedLevelUpText).text = Utils.ToCurrencyString(needGold);
        GetText(TextsType, (int)Texts.NeedLevelUpText).color = Utils.CoinCheck(needGold) ? Color.green : Color.red;
        GetText(TextsType, (int)Texts.GetExpText).text = $"<color=#00FF00>EXP</color> + {string.Format("{0:0.00}", Managers.PlayerM.NextExp())}%";

        GetText(TextsType, (int)Texts.CoinText).text = Utils.ToCurrencyString(Managers.GameM.Gold);

        CheckPlayer();
    }
    void CheckPlayer()
    {
        //GetImage(ImagesType, (int)Images.CharacterImage).sprite = "";
        GetText(TextsType, (int)Texts.CharacterLevelText).text = $"LV : {Managers.GameM.level}";
        //TODO :이거 소수점 끝까지 안나오게 수정하기
        GetText(TextsType, (int)Texts.UserCombatPowerText).text = Utils.ToCurrencyString(Utils.Datas.levelData.Damage(Managers.GameM.mPlayer.BaseDamage) + Utils.Datas.levelData.HP(Managers.GameM.mPlayer.BaseHp));
    }

    IEnumerator coPush()
    {
        yield return new WaitForSeconds(1f);
        isLevelUpButtonPush = true;
    }
    void StartSpawn()
    {
        Managers.SpawnM.Init();
        Managers.StageM.StateChange(Define.StageState.Ready);
    }

    Coroutine coroutine;
    void ExpUPAnim()
    {
        GetButton(ButtonsType, (int)Buttons.LevelUpButton).transform.DORewind();
        GetButton(ButtonsType, (int)Buttons.LevelUpButton).transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.25f);
        OnClickLevelupButton();
    }

    #endregion

    #region ItemPopup(TOP)

    public void SetHighGradeItem(Data.ItemData _data)
    {
        GetImage(ImagesType, (int)Images.ItemPopupFrameImage).sprite = Managers.ResourceM.GetAtlas(_data.ItemGrade.ToString());
        GetImage(ImagesType, (int)Images.ItemPopupItemImage).sprite = Managers.ResourceM.GetAtlas(_data.Name);
        GetText(TextsType, (int)Texts.ItemPopupText).text = Utils.StringToColorGrade(_data.ItemGrade) + _data.Description + "</color>을 획득하였습니다";

        PlayLegendaryPopupAnim();
    }
    public void PlayLegendaryPopupAnim()
    {

        GetObject(GameObjectsType, (int)GameObjects.ItemPopupObject).SetActive(true);


        ItemPopupObjectRect.localScale = new Vector3(0f, 1f, 1f);
        ItemPopupFrameObjectRect.localScale = Vector3.zero;

        var seq = DOTween.Sequence();
        seq.Append(ItemPopupObjectRect.DOScaleX(1.1f, 0.1f));
        seq.Append(ItemPopupObjectRect.DOScaleX(1.0f, 0.05f));

        seq.Append(ItemPopupFrameObjectRect.DOScale(new Vector3(1.1f, 0.9f, 1.0f), 0.1f));
        seq.Append(ItemPopupFrameObjectRect.DOScale(new Vector3(1f, 1.1f, 1f), 0.1f));
        seq.Append(ItemPopupFrameObjectRect.DOScale(Vector3.one, 0.05f));

        float startTime = 0.2f;

        seq.Insert(startTime, ItemPopupFrameObjectRect.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.1f));
        seq.Insert(startTime + 0.1f, ItemPopupFrameObjectRect.DOScale(new Vector3(1f, 1.1f, 1f), 0.05f));
        seq.Insert(startTime + 0.15f, ItemPopupFrameObjectRect.DOScale(Vector3.one, 0.05f));

        float displayTime = 2f;
        seq.AppendInterval(displayTime);
        seq.OnComplete(() =>
        {
            PlayLegendaryPopupCloseAnim();
        });

        seq.Play();


    }
    public void PlayLegendaryPopupCloseAnim()
    {
        var seq = DOTween.Sequence();


        seq.Append(ItemPopupFrameObjectRect.DOScale(new Vector3(1f, 1.1f, 1f), 0.05f));
        seq.Append(ItemPopupFrameObjectRect.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.1f));
        seq.Append(ItemPopupFrameObjectRect.DOScale(Vector3.zero, 0.1f));

        seq.Append(ItemPopupObjectRect.DOScaleX(1.1f, 0.1f));
        seq.Append(ItemPopupObjectRect.DOScaleX(0f, 0.15f));

        seq.OnComplete(() =>
                {
                    GetObject(GameObjectsType, (int)GameObjects.ItemPopupObject).SetActive(false);
                });

        seq.Play();
    }
    #endregion

    #region ItemPopup(Bottom)
    private Dictionary<TextMeshProUGUI, CancellationTokenSource> cancellationSources =
    new Dictionary<TextMeshProUGUI, CancellationTokenSource>();
    public void GetItem(Data.ItemData _data)
    {
        //TextMeshProUGUI newItemText = FindOldSlot();
        //if (newItemText == null)
        //{
        //    Debug.LogWarning("아이템 메시지 슬롯이 모두 꽉 찼습니다.");
        //    return;
        //}

        //foreach (var text in itemTexts)
        //{
        //    if (text.gameObject.activeSelf && text != newItemText)
        //    {
        //        RectTransform rects = text.GetComponent<RectTransform>();
        //        float targetY = rects.anchoredPosition.y + 50.0f;
        //        rects.DOKill();
        //        rects.anchoredPosition = new Vector2(0f, targetY);
        //    }
        //}


        //RectTransform rect = newItemText.GetComponent<RectTransform>();

        //rect.anchoredPosition = Vector2.zero;
        //newItemText.gameObject.SetActive(true);

        //newItemText.text = "아이템을 획득하였습니다 : " + Utils.StringToColorGrade(_data.ItemGrade) + "[" + _data.NameKR + "]</color>";
        //PlayTextFadeOut(newItemText).Forget();



        //if ((int)_data.ItemGrade >= (int)Define.ItemGrade.Rare)
        //{
        //    SetHighGradeItem(_data);

        //}

        //TODO : 이거 합쳐서 수정하기.
        bool AllActive = true;

        for (int i = 0; i < itemTexts.Count; i++)
        {
            if (itemTexts[i].gameObject.activeSelf == false)
            {
                itemTexts[i].gameObject.SetActive(true);
                itemTexts[i].text = "아이템을 획득하였습니다 : " + Utils.StringToColorGrade(_data.ItemGrade) + "[" + _data.NameKR + "]</color>";

                for (int j = 0; j < i; j++)
                {
                    RectTransform rect = itemTexts[j].GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + 50.0f);
                }

                AllActive = false;
                break;
            }
        }

        if (AllActive)
        {
            GameObject BaseRect = null;
            float yCount = 0.0f;
            for (int i = 0; i < itemTexts.Count; i++)
            {
                RectTransform rect = itemTexts[i].GetComponent<RectTransform>();
                if (rect.anchoredPosition.y > yCount)
                {
                    BaseRect = rect.gameObject;
                    yCount = rect.anchoredPosition.y;
                }
            }

            for (int i = 0; i < itemTexts.Count; i++)
            {
                if (BaseRect == itemTexts[i].gameObject)
                {
                    itemTexts[i].gameObject.SetActive(false);
                    itemTexts[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);

                    itemTexts[i].gameObject.SetActive(true);
                    itemTexts[i].text = "아이템을 획득하였습니다 : " + Utils.StringToColorGrade(_data.ItemGrade) + "[" + _data.NameKR + "]</color>";

                    
                }
                else
                {
                    RectTransform rect = itemTexts[i].GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(0.0f, rect.anchoredPosition.y + 50.0f);
                }
            }
        }

        if ((int)_data.ItemGrade >= (int)Define.ItemGrade.Rare)
        {
            SetHighGradeItem(_data);

        }
    }

    public TextMeshProUGUI FindOldSlot()
    {
        foreach(var text in itemTexts)
        {
            if (!text.gameObject.activeSelf) return text;
        }

        TextMeshProUGUI oldestText = null;
        float maxY = float.MinValue;

        foreach (var text in itemTexts)
        {
            if (!text.gameObject.activeSelf) continue;
            
            RectTransform rect = text.GetComponent<RectTransform>();
            if(rect.anchoredPosition.y > maxY)
            {
                maxY = rect.anchoredPosition.y;
                oldestText = text;
            }
        }
        
        if(oldestText != null)
        {
            CancellationTokenSource ctsToCancel = null;
            if(cancellationSources.TryGetValue(oldestText, out ctsToCancel))
            {
                cancellationSources.Remove(oldestText);
            }
            if(ctsToCancel != null)
            {
                ctsToCancel.Cancel();
                ctsToCancel.Dispose();
            }

            oldestText.DOKill(true);
            RectTransform rect = oldestText.GetComponent<RectTransform>();
            
            rect.anchoredPosition = Vector2.zero;
            rect.gameObject.SetActive(false);
            

            return oldestText;
        }

        return null;
    }
    public async UniTask PlayTextFadeOut(TextMeshProUGUI _text)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        cancellationSources[_text] = cts;

        RectTransform rect = _text.GetComponent<RectTransform>();
        Color startColor = _text.color;
        startColor.a = 1f;
        _text.color = startColor;

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f), ignoreTimeScale: false, cancellationToken: token);
            await _text.DOFade(0f, 0.5f).ToUniTask(cancellationToken: token);
        }
        catch(OperationCanceledException)
        {
            Color cancelColor = _text.color;
            cancelColor.a = 0;
            _text.color = cancelColor;
        }
        finally
        {
            if (cancellationSources.ContainsKey(_text))
            {
                cancellationSources.Remove(_text);
                cts.Dispose(); // 중요!
            }

            if (rect != null)
            {
                rect.gameObject.SetActive(false);
                rect.anchoredPosition = Vector2.zero;
            }
        }
    }

    #endregion

    #region LevelUpButton
    float timer = 0f;
    bool isLevelUpButtonPush = false;
    public void Tick(float _deltaTime)
    {
        if (isLevelUpButtonPush)
        {
            timer += _deltaTime;
            if (timer >= 0.01f)
            {
                timer = 0.0f;
                ExpUPAnim();
            }

        }
    }
    #endregion

    #region FadeInOut
    //TODO : 그냥 const로 만들어버릴까
    float fadeDuration = 1;
    public async UniTask AsyncFadeInOut(bool _isFadeIn, bool _isSibling = false)
    {
        Image fadeImage = GetImage(ImagesType, (int)Images.FadeImage);

        if (_isSibling)
        {

            fadeImage.transform.parent = Managers.UIM.Root.transform;
            fadeImage.transform.SetAsLastSibling();
        }
        else
        {

            fadeImage.transform.parent = this.transform;
            fadeImage.transform.SetSiblingIndex(0);
        }

        await CoFadeInOutAsync(_isFadeIn);

    }

    private async UniTask CoFadeInOutAsync(bool _isFadeIn)
    {
        Image fadeImage = GetImage(ImagesType, (int)Images.FadeImage);

        float current = 0.0f;
        float percent = 0.0f;
        float start = _isFadeIn ? 1.0f : 0.0f;
        float end = _isFadeIn ? 0.0f : 1.0f;
        fadeImage.raycastTarget = true;

        while (percent < 1.0f)
        {
            current += Time.deltaTime;
            percent = current / fadeDuration;
            float LerpPos = Mathf.Lerp(start, end, percent);
            fadeImage.color = new Color(0, 0, 0, LerpPos);

            await UniTask.Yield();
        }

        fadeImage.raycastTarget = false;
    }
    #endregion
}
