using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour, ITickable
{
    //버프를 소유한 객체
    private CreatureController owner;
    //적용중인 버프 목록
    private List<IBuff> activeBuffs = new List<IBuff>();

    private void Awake()
    {
        owner = GetComponent<CreatureController>();
        if (owner == null)
        {
            Debug.LogError("CreatureController 없음 !");
        }
        Managers.UpdateM.Register(this);
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
        //같은 종류의 버프가 있으면 제거하고 다시시작.
        IBuff existingBuff = activeBuffs.Find(b => b.GetBuffType() == _newBuff.GetBuffType());
        if(existingBuff != null)
        {
            RemoveBuff(existingBuff);
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
        }
    }

}
