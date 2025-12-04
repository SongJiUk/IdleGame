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

        //떄리던쪽 때리는게 맞을거같아서 이게 나을듯
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
        //TODO : ��ų ������ �����ͼ� ������ �̸� �־
        var effect = Managers.ResourceM.Instantiate("KnightSkill", _pooling: true);
        effect.transform.position = owner.transform.position;

    }
}
