using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


//공통 ?��?�� ?��미�?? 계산
public class DamageEffect : ISkillEffect
{
    double damageMultiplier;
    double splashRadius;
    double splashMultiplier;
    float delayTime;

    public DamageEffect(double _damageMultiplier, double _splashRadius = 0)
    {
        damageMultiplier = _damageMultiplier;
        splashRadius = _splashRadius;
        splashMultiplier = _damageMultiplier / 3;
    }

    public void Execute(CreatureController _caster, CreatureController _target, float _delayTime = 0f)
    {
        if (_target == null) return;

        if (_delayTime > 0)
        {
            DelayDamage(_caster, _target, _delayTime).Forget();
        }
        else
        {
            InstantDamage(_caster, _target);
        }
        
    }

    public void ApplySplashDamage(CreatureController _caster, CreatureController _target, double _radius, double _multiplier)
    {
        List<MonsterController> mList = Managers.ObjectM.mcList;

        double splashDamage = _caster.Damage * _multiplier;
        Vector3 centerPos = _target.transform.position;

        foreach (MonsterController monster in new List<CreatureController>(mList))
        {
            if (monster == null || monster.IsDead) continue;
            if (monster == _target) continue;

            Vector3 monsterPos = monster.transform.position;

            float distance = Vector3.Distance(centerPos, monsterPos);

            if (distance <= _radius)
                monster.GetDamage(splashDamage, _caster, _isSkill: true);
        }
    }

    public void InstantDamage(CreatureController _caster, CreatureController _target)
    {
        if (_target.IsDead) return;

        double baseDamage = _caster.Damage * damageMultiplier;
        _target.GetDamage(baseDamage, _caster, _isSkill : true);


        if (splashRadius > 0 && splashMultiplier > 0)
        {
            ApplySplashDamage(_caster, _target, splashRadius, splashMultiplier);
        }

    }
    public async UniTask DelayDamage(CreatureController _caster, CreatureController _target, float _delayTime)
    {
        await UniTask.WaitForSeconds(_delayTime);
        if (_target == null || _target.IsDead) return;

        InstantDamage(_caster, _target);

    }



}
