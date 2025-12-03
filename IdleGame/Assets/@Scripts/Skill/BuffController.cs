using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour, ITickable
{
    private CreatureController owner;
    private List<IBuff> activeBuffs = new List<IBuff>();

    private void Awake()
    {
        owner = GetComponent<CreatureController>();
        if (owner == null)
        {
            Debug.LogError("CreatureController ¾øÀ½");
        }
    }

    public void Tick(float _deltaTime)
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            IBuff buff = activeBuffs[i];
            buff.Update(_deltaTime);

            if (buff.isExpired())
            {
                RemoveBuff(buff);
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
        if (activeBuffs.Contains(_buff))
        {
            _buff.Remove(owner);
            activeBuffs.Remove(_buff);


            if (activeBuffs.Count == 0)
            {
                Managers.UpdateM.UnRegister(this);
            }
        }
    }

}
