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
        //이 객체를 참조로 설정한다.
        owner = GetComponent<CreatureController>();

    }


    public void InitSkills(List<SkillBase> _skills)
    {
        skills.Clear();
        if(_skills != null)
        {
            skills.AddRange(_skills);
        }

        Debug.Log($"{owner.gameObject.name}의 스킬 {skills.Count}개가 초기화되었습니다.");
    }

    //해당 스킬을 사용
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
