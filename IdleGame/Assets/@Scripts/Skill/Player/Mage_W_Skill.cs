using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage_W_Skill : SkillBase
{
    BuffEffect attackBuffEffect;
    BuffEffect defenseBuffEffect;
    public Mage_W_Skill()
    {
        SetUpEffect();
    }

    protected override void SetUpEffect()
    {
        //TODO : 스킬 정보 가져와서 여기에 넣기
        //타겟에게 10초동안 attackBuff적용 효과
        

    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        Managers.DataM.SkillDataDic.TryGetValue(_caster.DATA.SkillDataID, out Data.SkillData skilldata);
        float duration = 0;
        foreach(int data in skilldata.BuffList_ID)
        {
            Managers.DataM.SkillEffectDataDic.TryGetValue(data, out var buffData);
            duration = buffData.Duration;
        }
        
        //TODO : 이거 공격력, 방어력 버프 이펙트도 나눠야될듯
        attackBuffEffect = new BuffEffect(_duration => new AttackBuff(_duration), duration);
        defenseBuffEffect = new BuffEffect(_duration => new DefenseBuff(_duration), duration);


        CreatureController randPlayer = Utils.FindRandomPlayer(_caster); ;

        if (randPlayer != null)
        {
            BuffEffect chosenEffect = (Random.Range(0, 2) == 0) ? attackBuffEffect : defenseBuffEffect;

            chosenEffect.Execute(_caster, randPlayer);

            foreach(var data in skilldata.TargetVFX_ID)
            {
                string vfxName = Utils.GetVfxPrefabName(data);
                if (!string.IsNullOrEmpty(vfxName))
                {
                    var effect = Managers.ResourceM.Instantiate(vfxName, _pooling: true);
                    effect.transform.position = _caster.transform.position;
                }

            }
            //TODO : 바꾸기..

            return true;
        }
        else
        {
            Debug.Log("[Mage_W_Skill] : 유효한 아군이 없음");
            return false;
        }

        //TODO : 쿨타임 처리(메인 캐릭터일떄)
    }
}
