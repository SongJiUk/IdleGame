using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.SearchService;
using UnityEngine;

public class MeleeAttackController : ProjectileController
{
    ParticleSystem particle;
    public override void AttackInit(MonsterController _mc, double _dmg, string _characterName = "")
    {
        base.AttackInit(_mc, _dmg, _characterName);
        target = _mc;
        if (target != null && !target.IsDead)
        {
            target.GetDamage(_dmg);

            isHit = true;

            //TODO : 수정
            if (particle == null)
            {
                GameObject go = Managers.ResourceM.Instantiate("MeleeAttack", _pooling: true);
                particle = go.GetOrAddComponent<ParticleSystem>();
            }
            particle.transform.position = target.transform.position;
            particle.Play();
            ReturnObject(particle.main.duration).Forget();
        }

        Managers.ObjectM.DeSpawn(this);

    }


}
