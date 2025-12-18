using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI
;

public class UI_RecallPopup : UI_Popup
{
    const int horizontal_limit = 4;
    const int horizontalCount = 3;
    Transform[] horizontals = new Transform[horizontalCount];

    enum GameObjects
    {
        Horizontal_1,
        Horizontal_2,
        Horizontal_3,
    }


    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        GameObjectsType = typeof(GameObjects);

        BindObject(GameObjectsType);


        for (int i = 0; i < horizontalCount; i++)
        {
            horizontals[i] = GetObject(GameObjectsType, (int)GameObjects.Horizontal_1 + i).transform;
        }

        return true;
    }

    public async void GetRecallHero(int _count)
    {
        await RecallHeroes(_count);

    }


    async UniTask RecallHeroes(int _count)
    {
        int horizontalCount = 0;

        for (int i = 0; i < _count; i++)
        {
            float percentage = Random.Range(0.0f, 100.0f);
            float r_Percentage = 0.0f;
            Define.CharacterGrade grade = Define.CharacterGrade.Common;

            if (i % horizontal_limit == 0 && i != 0)
            {
                horizontalCount++;
            }

            var heroInfo = Managers.UIM.MakeSubItem<UI_RecallHeroIcon>(horizontals[horizontalCount]);

            for (int j = 0; j < 5; j++)
            {
                r_Percentage += Utils.Gacha_Percentage[j];
                if (percentage <= r_Percentage)
                {
                    grade = (Define.CharacterGrade)j;
                    break;
                }
            }
            Data.CreatureData data = Managers.GameM.gameData.GetGradeCharacter(grade);

            //TODO : 여기에 가챠 정보 들어가야함.
            heroInfo.Init().Forget();
            heroInfo.SetHeroIcon(data);

            await UniTask.Delay(100);
        }
    }

}
