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

    void Start()
    {
        //스킬 초기화(조립해줘야함)
        Archer_Skill archer_Skill = new Archer_Skill();
        skills.Add(archer_Skill);
    }

    public void UseSKill(int _skillIndex, CreatureController _target)
    {
        if (_skillIndex < 0 || _skillIndex >= skills.Count)
        {
            Debug.LogError("해당 인덱스에 스킬 없음");
            return;
        }

        SkillBase useSkill = skills[_skillIndex];

        useSkill.UseSkill(owner, _target);

    }
}
