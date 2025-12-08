using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour, ITickable
{
    //버프를 받는 오브젝트
    private CreatureController owner;
    private List<IBuff> activeBuffs = new List<IBuff>();

    private void Awake()
    {
        owner = GetComponent<CreatureController>();
        if (owner == null)
        {
            Debug.LogError("CreatureController ����");
        }
    }

    public void Tick(float _deltaTime)
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {   //TODO : 아오!!!
            IBuff buff = activeBuffs[i];
            if (i >= activeBuffs.Count) continue;

            buff.Update(_deltaTime);



            if (buff.isExpired())
            {
                buff.Remove(owner);
                activeBuffs.RemoveAt(i);
                if (activeBuffs.Count == 0)
                {
                    Managers.UpdateM.UnRegister(this);
                }
            }
        }
    }

    public void AddBuff(IBuff _newBuff)
    {
        IBuff existingBuff = activeBuffs.Find(b => b.GetBuffType() == _newBuff.GetBuffType());
        if (existingBuff != null)
        {
            RemoveBuff(existingBuff);
        }

        if (activeBuffs.Count == 0)
        {
            Managers.UpdateM.Register(this);
        }

        activeBuffs.Add(_newBuff);
        _newBuff.Apply(owner);
    }

    public void RemoveBuff(IBuff _buff)
    {
        int index = activeBuffs.IndexOf(_buff);

        if (index != -1)
        {
            _buff.Remove(owner);
            activeBuffs.RemoveAt(index);

            if (activeBuffs.Count == 0)
            {
                Managers.UpdateM.UnRegister(this);
            }
        }
        // if (activeBuffs.Contains(_buff))
        // {
        //     _buff.Remove(owner);
        //     activeBuffs.Remove(_buff);


        //     if (activeBuffs.Count == 0)
        //     {
        //         Managers.UpdateM.UnRegister(this);
        //     }
        // }
    }

    public void ClearAllBuffs()
    {
        if (activeBuffs.Count > 0)
        {
            Managers.UpdateM.UnRegister(this);
        }
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            IBuff buff = activeBuffs[i];
            buff.Remove(owner);
            activeBuffs.RemoveAt(i);
        }

        activeBuffs.Clear();
    }
}
