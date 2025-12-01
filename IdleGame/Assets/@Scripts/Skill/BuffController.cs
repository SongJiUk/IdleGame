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
        //TODO : 같은 종류의 버프가 있다면, 중첩? or 시간만 갱신?

        activeBuffs.Add(_newBuff);
        //TODO : 여기서 버프를 적용해줄 타겟을 찾아야함
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
