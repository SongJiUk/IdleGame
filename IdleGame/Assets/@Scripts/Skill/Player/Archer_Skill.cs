using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class Archer_Skill : SkillBase
{
    public Archer_Skill() { }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        InitSkillData(_caster);
        CreatureController randTarget = _target;
        if(randTarget == null || randTarget.IsDead) 
        {
            randTarget = Utils.FindRandomEnemyInRange(_caster, _caster.DATA.AttackRange * 2);
        }

        if (randTarget == null || randTarget.IsDead)
        {
            Debug.Log("아처 스킬 타겟이 없음");
            return false;
        }

        _caster.transform.LookAt(randTarget.transform);
        SpawnProjectileDelay(_caster, randTarget).Forget();
        ResetSkillStateAsync(_caster, anim_Duration).Forget();

        return true;

    }

    async UniTaskVoid SpawnProjectileDelay(CreatureController _caster, CreatureController _target)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: _caster.GetCancellationTokenOnDestroy());

            if (_target == null || _target.IsDead) return;

            var projectile = Managers.ObjectM.Spawn<RangeAttackController>(_caster.transform.position,
               skill_ProjectileID,
               _caster,
               _target,
               true);

            projectile.OnHitTarget = () =>
            {
                foreach (var effect in effects)
                {
                    effect.Execute(_caster, _target);
                }
                ShowEffect(_target);
            };
            

        }
        catch (OperationCanceledException) { }
        catch (Exception e) { }
    }
}
