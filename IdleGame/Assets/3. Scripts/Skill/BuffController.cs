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
        if (activeBuffs.Count == 0)
        {
            Managers.UpdateM.UnRegister(this);
            return;
        }

        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (i >= activeBuffs.Count)
            {
                if (activeBuffs.Count == 0) return;

                continue;
            }

            IBuff buff = activeBuffs[i];
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


    public float GetTotalRatio(Define.BuffEffectType _type)
    {
        float total = 0;
        foreach (var buff in activeBuffs)
        {
            if (buff is BuffBase b && b.GetBuffTypes() == _type)
            {
                float r = b.GetRatio();
                // 데이터가 0이면 계산에서 제외하거나 1로 취급
                if (r <= 0)
                {
                    Debug.LogWarning($"{b.GetType().Name}의 Ratio가 0입니다! 데이터를 확인하세요.");
                    continue;
                }
                total += (r - 1f);
            }
        }

        if (total != 0)
        {
            //Debug.Log($"<color=yellow>[Buff_Step 3]</color> {owner.name} 스탯 계산 중 ({_type}): 총 합산 비율 {total * 100}%");
        }
        return total;
    }


    public void AddBuff(IBuff _newBuff)
    {
        //Debug.Log($"<color=cyan>[Buff_Step 1]</color> {_newBuff.GetType().Name} 추가 시도 (대상: {owner.name})");

        IBuff existingBuff = activeBuffs.Find(b => b.GetBuffType() == _newBuff.GetBuffType());
        if (existingBuff != null)
        {
            //Debug.Log($"<color=gray>[Buff_Notice]</color> 기존 동일 버프가 있어 갱신합니다.");
            RemoveBuff(existingBuff);
        }

        if (activeBuffs.Count == 0)
        {
            Managers.UpdateM.Register(this);
        }

        activeBuffs.Add(_newBuff);
        _newBuff.Apply(owner);
        //Debug.Log($"<color=magenta>[Buff_Step 2]</color> 현재 {owner.name}의 활성 버프 수: {activeBuffs.Count}");

    }

    public void RemoveBuff(IBuff _buff)
    {
        int index = activeBuffs.IndexOf(_buff);

        if (index != -1)
        {
            //Debug.Log($"<color=orange>[Buff_End]</color> {_buff.GetType().Name} 만료되어 제거됨");
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
