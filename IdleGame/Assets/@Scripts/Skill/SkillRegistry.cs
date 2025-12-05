using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
public static class SkillRegistry
{
    private static readonly Dictionary<CreatureType, List<Type>> CreatureSkillDic
       = new Dictionary<CreatureType, List<Type>>()
       {
           {
               CreatureType.Cleric,
               new List<Type>{typeof(Cleric_Skill)}
           },

           {
               CreatureType.Archer,
               new List<Type>{typeof(Archer_Skill)}
           },

           {
               CreatureType.Assassin,
               new List<Type>{typeof(Assassin_Skill) }
           },
           {
               CreatureType.Hammer,
               new List<Type>{typeof(Hammer_Skill) }
           },
           {
               CreatureType.Knight,
               new List<Type>{typeof(Knight_Skill) }
           },
           {
               CreatureType.SpearMan,
               new List<Type>{typeof(SpearMan_Skill) }
           },
           {
               CreatureType.TwoHandSword,
               new List<Type>{typeof(TwoHandSword_Skill) }
           },
           {
               CreatureType.Mage_M,
               new List<Type>{typeof(Mage_M_Skill) }
           },
           {
               CreatureType.Mage_W,
               new List<Type>{typeof(Mage_W_Skill) }
           },
           {
               CreatureType.Boss,
               new List<Type>{typeof(Boss_Electric) }
           },


       };


    public static List<SkillBase> CreateSkillsForCreature(CreatureType _type)
    {
        List<SkillBase> skills = new List<SkillBase>();

        if (CreatureSkillDic.TryGetValue(_type, out List<Type> skillTypes))
        {
            foreach (Type skillType in skillTypes)
            {
                try
                {
                    SkillBase newSkill = (SkillBase)Activator.CreateInstance(skillType);
                    if (newSkill == null)
                    {
                        Debug.LogError($"[SkillRegistry] 스킬 생성 실패 : {skillType.Name}");
                        continue;
                    }

                    skills.Add(newSkill);
                }
                catch (MissingMethodException)
                {
                    Debug.LogError($"[SkillRegistry] {skillType}에 매개변수 없는 생성자가 없음");
                }
                catch (Exception e)
                {
                    Debug.LogError("스킬 인스턴스 생성 오류");
                }
            }
        }
        else
        {
            Debug.LogError("Dictinary에 저장된 값이 없음.");
        }

        return skills;
    }
}
