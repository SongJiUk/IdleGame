using System.Collections;
using System.Collections.Generic;
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


    public void UseSKill(int _skillIndex = 0, CreatureController _target = null)
    {
        if (_skillIndex < 0 || _skillIndex >= skills.Count)
        {
            Debug.LogError("[SkillController] �ش� ��ų�� �����ϴ�.");
            return;
        }

        SkillBase useSkill = skills[_skillIndex];
        CreatureController caster = owner;

        caster.AnimatorChange(Define.CreatureState.Skill);
        ShowEffect();
        //떄리던쪽 때리는게 맞을거같아서 이게 나을듯
        useSkill.UseSkill(owner, _target);
    }

    public void ShowEffect()
    {
        //TODO : ��ų ������ �����ͼ� ������ �̸� �־
        var effect = Managers.ResourceM.Instantiate("KnightSkill", _pooling: true);
        effect.transform.position = owner.transform.position;

    }
}
