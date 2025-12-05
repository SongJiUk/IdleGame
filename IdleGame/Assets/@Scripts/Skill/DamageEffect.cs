using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//공통 ?��?�� ?��미�?? 계산
public class DamageEffect : ISkillEffect
{
    double damageMultiplier;
    double splashRadius;
    double splashMultiplier;


    public DamageEffect(double _damageMultiplier, double _splashRadius = 0)
    {
        damageMultiplier = _damageMultiplier;
        splashRadius = _splashRadius;
        splashMultiplier = _damageMultiplier / 3;
    }

    public void Execute(CreatureController _caster, CreatureController _target)
    {
        if (_target == null) return;

        double baseDamage = _caster.Damage * damageMultiplier;
        _target.GetDamage(baseDamage, _caster);


        if (splashRadius > 0 && splashMultiplier > 0)
        {
            ApplySplashDamage(_caster, _target.transform.position, splashRadius, splashMultiplier);
        }
    }

    public void ApplySplashDamage(CreatureController _caster, Vector3 _center, double _radius, double _multiplier)
    {
        List<MonsterController> mList = Managers.ObjectM.mcList;

        double splashDamage = _caster.Damage * _multiplier;

        foreach (MonsterController monster in mList)
        {
            Vector3 monsterPos = monster.transform.position;

            float distance = Vector3.Distance(_center, monsterPos);

            if (distance <= _radius)
                monster.GetDamage(splashDamage, _caster);
        }
    }



}
