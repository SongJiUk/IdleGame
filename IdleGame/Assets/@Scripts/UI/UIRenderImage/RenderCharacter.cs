using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCharacter : MonoBehaviour
{
    public Transform[] circles;
    public GameObject[] particles;
    bool[] isGetCharacter = new bool[7];

    GameObject[] useCharacters = new GameObject[7];
    public bool IsUseCharacter(int _value)
    {
        return isGetCharacter[_value];
    }
    public void GetRenderCharacterParitcle(bool _isUseSpawnPoint)
    {

        for (int i = 0; i < particles.Length; i++)
        {
            if (isGetCharacter[i]) continue;

            particles[i].SetActive(_isUseSpawnPoint);
        }
    }

    public void InitCharacter()
    {
        for (int i = 0; i < Managers.CharacterM.Characters.Length; i++)
        {
            if (Managers.CharacterM.Characters[i] != null && !isGetCharacter[i])
            {
                isGetCharacter[i] = true;
                string name = Managers.CharacterM.Characters[i].data.Name;
                var go = Managers.ResourceM.Instantiate(name, _pooling: true);
                go.transform.position = circles[i].position;
                go.GetComponent<PlayerController>().enabled = false;
                useCharacters[i] = go;
            }

        }
    }

    public void ChangeCharacter()
    {
        for (int i = 0; i < Managers.CharacterM.Characters.Length; i++)
        {
            if (useCharacters[i] != null)
            {
                Managers.ResourceM.Destroy(useCharacters[i]);
                useCharacters[i] = null;

                string name = Managers.CharacterM.Characters[i].data.Name;
                var go = Managers.ResourceM.Instantiate(name, _pooling: true);
                go.transform.position = circles[i].position;
                go.GetComponent<PlayerController>().enabled = false;
                useCharacters[i] = go;
            }
        }
    }

    public void RemoveCharacter()
    {
        for (int i = 0; i < Managers.CharacterM.Characters.Length; i++)
        {
            if (Managers.CharacterM.Characters[i] == null && isGetCharacter[i])
            {
                isGetCharacter[i] = false;
                Managers.ResourceM.Destroy(useCharacters[i]);
                useCharacters[i] = null;
            }
        }
    }


    public bool isCheckCharacter(int _index)
    {
        if (isGetCharacter[_index]) return true;


        return false;
    }
}
