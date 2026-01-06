using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
public class RangeAttackController : ProjectileController
{
    Dictionary<string, GameObject> Projectiles = new Dictionary<string, GameObject>();
    Dictionary<string, ParticleSystem> Muzzles = new Dictionary<string, ParticleSystem>();
    CreatureController owner;
    bool isSkillProjectile = false;

    private void Awake()
    {
        Transform projectiles = transform.GetChild(0);
        Transform muzzles = transform.GetChild(1);

        for (int i = 0; i < projectiles.childCount; i++)
            Projectiles.Add(projectiles.GetChild(i).name, projectiles.GetChild(i).gameObject);


        for (int i = 0; i < muzzles.childCount; i++)
            Muzzles.Add(muzzles.GetChild(i).name, muzzles.GetChild(i).GetComponent<ParticleSystem>());
    }

    public override void AttackInit(CreatureController _cc, double _dmg, CreatureController _owner, bool _isSkillProjectile = false)
    {
        if (_cc == null || !_cc.gameObject.activeInHierarchy)
        {
            Managers.ObjectM.DeSpawn(this);
            return;
        }

        owner = _owner;
        isSkillProjectile = _isSkillProjectile;
        base.AttackInit(_cc, _dmg, owner);
        //TODO : 여기에서 null값뜸 똑같은 상황나오면 체크하기
        if (target == null)
        {
            Managers.ObjectM.DeSpawn(this);
            return;
        }
        transform.LookAt(target.transform);


        targetPos = target.transform.position;
        targetPos.y = 0.5f;

        Vector3 startPos = transform.position;
        startPos.y = 0.5f;
        transform.position = startPos;

        if (Projectiles.Count != 0) Projectiles[characterName].SetActive(true);
    }

    public override void Tick(float _deltaTime)
    {
        if (isHit) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, _deltaTime * 5f);

        if (Vector3.Distance(transform.position, targetPos) <= 0.1f)
        {
            if (target != null)
            {
                isHit = true;
                target.GetDamage(damage, owner, _isSkill: isSkillProjectile);


                if (Projectiles.Count != 0) Projectiles[characterName].SetActive(false);
                if (Muzzles.Count != 0) Muzzles[characterName].Play();

                if (Muzzles.Count != 0) ReturnObject(Muzzles[characterName].duration).Forget();
                else ReturnObject(0.3f).Forget();
            }
        }
    }
}
