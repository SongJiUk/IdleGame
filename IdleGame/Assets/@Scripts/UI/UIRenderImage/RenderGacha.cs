using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderGacha : MonoBehaviour
{
    public GameObject elevenObject;
    public GameObject oneObject;
    public Transform[] elevenCircles;
    public Transform oneCircle;


    [SerializeField] GameObject LootEffect;
    [SerializeField] GameObject[] GradeEffect;
    List<GameObject> CharacterList = new List<GameObject>();
    List<GameObject> EffectList = new List<GameObject>();

    public void GetHerosForEleven(int _value, Data.CreatureData _data)
    {
        var go = Managers.ResourceM.Instantiate(_data.PrefabName, _pooling:true);
        go.transform.position = elevenCircles[_value].position;
        go.GetComponent<PlayerController>().enabled = false;
        CharacterList.Add(go);

        string effectName = "Gacha" + _data.CharacterGrade.ToString();
        var effect = Managers.ResourceM.Instantiate(effectName, _pooling: true);
        effect.transform.position = elevenCircles[_value].position;
        EffectList.Add(effect);
    }

    public void GetHero(Data.CreatureData _data)
    {
        var go = Managers.ResourceM.Instantiate(_data.PrefabName, _pooling: true);
        go.transform.position = oneCircle.position;
        CharacterList.Add(go);

        string effectName = "Gacha" + _data.CharacterGrade.ToString();
        var effect = Managers.ResourceM.Instantiate(effectName, _pooling: true);
        effect.transform.position = oneCircle.position;
        EffectList.Add(effect);
    }

    public void ClearList()
    {
        for(int i=0; i<CharacterList.Count; i++)
        {
            Managers.ResourceM.Destroy(CharacterList[i].gameObject);
        }

        for (int i = 0; i < EffectList.Count; i++)
        {
            Managers.ResourceM.Destroy(EffectList[i].gameObject);
        }

        CharacterList.Clear();
        EffectList.Clear();
    }
}
