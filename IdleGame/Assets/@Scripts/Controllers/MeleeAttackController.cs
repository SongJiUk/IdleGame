using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.SearchService;
using UnityEngine;

public class MeleeAttackController : ProjectileController
{
    ParticleSystem particle;
    public override void AttackInit(CreatureController _cc, double _dmg, CreatureController _owner = null, bool _isSkillProjectile = false)
    {
        base.AttackInit(_cc, _dmg, _owner);
        target = _cc;
        if (target != null && !target.IsDead)
        {
            target.GetDamage(_dmg, _owner);

            isHit = true;

            //TODO : 보스에서 사망했거나 스테이지를 못깨서 사망했을때, 몬스터가 플레이어 못때리게 설정해뒀으니까 이것도 안되게
            if (Managers.StageM.isDead && !_owner.IsPlayer)
            {
                Managers.ObjectM.DeSpawn(this);
                return;
            }


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
