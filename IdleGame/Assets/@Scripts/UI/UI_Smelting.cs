using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Smelting : UI_Base
{
    #region Enum

    enum Texts
    {
        SmeltingNameText,
        SmeltingProbabilityText
    }
    #endregion
    Data.SmeltData data;
    Transform parent;
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        TextsType = typeof(Texts);

        BindText(TextsType);


        return true;
    }

    public void SetInfo(Data.SmeltData _data, Transform _parent)
    {

        data = _data;
        parent = _parent;



        float min = data.Min;
        float max = data.Max;

        float randStat = UnityEngine.Random.Range(min, max);
        float statValue = Mathf.Round(randStat * 100f) / 100f;

        GetText(TextsType, (int)Texts.SmeltingNameText).text = Utils.StringToColorGrade(data.Grade) + data.Description + "</color>";

        GetText(TextsType, (int)Texts.SmeltingProbabilityText).text = Utils.StringToColorGrade(data.Grade) + statValue.ToString() + "</color>"; ;

    }

}
