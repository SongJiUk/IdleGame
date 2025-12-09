using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    List<SkillBase> skills = new List<SkillBase>();
    private CreatureController owner;

    private void Awake()
    {
        owner = GetComponent<CreatureController>();

    }


    public void InitSkills(List<SkillBase> _skills)
    {
        skills.Clear();
        if (_skills != null)
        {
            skills.AddRange(_skills);
        }

        if (owner != null)
        {
            foreach (var skill in skills)
            {
                skill.SetSkill(owner);
            }
        }
    }


    public bool UseSKill(int _skillIndex = 0, CreatureController _target = null)
    {
        if (_skillIndex < 0 || _skillIndex >= skills.Count)
        {
            Debug.LogError("[SkillController]해당 인덱스의 스킬이 없음.");
            return false;
        }

        SkillBase useSkill = skills[_skillIndex];
        CreatureController caster = owner;

        bool skillExecuted = useSkill.UseSkill(owner, _target);

        if (skillExecuted)
        {
            caster.AnimatorChange(Define.CreatureState.Skill);
            ShowEffect();
        }
        else
        {
            Debug.Log("[SkillController]스킬이 실행되지않음 ");
            return false;
        }

        return true;
    }



    public void ShowEffect()
    {
        if (!Managers.DataM.SkillDataDic.TryGetValue(owner.DATA.SkillDataID, out var skillData))
        {
            Debug.LogError($"스킬 ID {owner.DATA.SkillDataID} 데이터 없음.");
            return;
        }


        foreach (var data in skillData.CastingVFX_ID)
        {
            string vfxName = Utils.GetVfxPrefabName(data);
            if (!string.IsNullOrEmpty(vfxName))
            {
                var effect = Managers.ResourceM.Instantiate(vfxName, _pooling: true);


                //TODO : 이거 y값 높여주면 다른 ui보다 위여서 안겹쳐서 보임
                Vector3 pos = owner.transform.position;
                pos.y = effect.transform.position.y;

                effect.transform.position = pos;
                effect.transform.rotation = Quaternion.Euler(
                                            effect.transform.eulerAngles.x,
                                            owner.transform.eulerAngles.y,
                                            effect.transform.eulerAngles.z);
            }
        }
    }


    public void ClearAllSkillsVFX()
    {
        foreach (var skill in skills)
        {
            skill.ClearSkillVFX();
        }
    }
}
