using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ê³µí†µ ?Š¤?‚¬ ?°ë¯¸ì?? ê³„ì‚°
public class DamageEffect : ISkillEffect
{
    double damageMultiplier;
    double splashRadius = 0f;
    double splashMultiplier = 0f;

    public DamageEffect(double _damageMultiplier, double _splashRadius = 0, double _splashMultiplier = 0)
    {
        damageMultiplier = _damageMultiplier;
        splashRadius = _splashRadius;
        splashMultiplier = _splashMultiplier;
    }

    public void Excute(CreatureController _caster, CreatureController _target)
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
        List<MonsterController> mclist = Managers.ObjectM.mcList;

        double splashDamage = _caster.Damage * _multiplier;

        foreach (MonsterController monster in mclist)
        {
            Vector3 monsterPos = monster.transform.position;

            float distance = Vector3.Distance(_center, monsterPos);

            if (distance <= _radius)
                monster.GetDamage(splashDamage, _caster);
        }
    }



}
