using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class RangeAttackController : ProjectileController
{
    Dictionary<string, GameObject> Projectiles = new Dictionary<string, GameObject>();
    Dictionary<string, ParticleSystem> Muzzles = new Dictionary<string, ParticleSystem>();
    CreatureController owner;


    //TODO: ?´? ‡ê²? ?•˜?Š”ê²? ë§ëŠ”ê±´ê?? ?‹¶ê¸´í•œ?° ?‚˜ì¤‘ì— ë¦¬í™?† ë§í• ?•Œ ?ˆ˜? •?•˜?.
    private void Awake()
    {
        Transform projectiles = transform.GetChild(0);
        Transform muzzles = transform.GetChild(1);

        for (int i = 0; i < projectiles.childCount; i++)
            Projectiles.Add(projectiles.GetChild(i).name, projectiles.GetChild(i).gameObject);


        for (int i = 0; i < muzzles.childCount; i++)
            Muzzles.Add(muzzles.GetChild(i).name, muzzles.GetChild(i).GetComponent<ParticleSystem>());
    }

    public override void AttackInit(CreatureController _cc, double _dmg, CreatureController _owner)
    {
        owner = _owner;
        base.AttackInit(_cc, _dmg, owner);
        transform.LookAt(target.transform);
        targetPos = target.transform.position;
        Projectiles[characterName].SetActive(true);
    }

    public override void Tick(float _deltaTime)
    {
        if (isHit) return;

        targetPos.y = 0.5f;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, _deltaTime * 5f);

        if (Vector3.Distance(transform.position, targetPos) <= 0.1f)
        {
            if (target != null)
            {
                isHit = true;
                target.GetDamage(damage, owner);


                Projectiles[characterName].SetActive(false);
                Muzzles[characterName].Play();

                ReturnObject(Muzzles[characterName].duration).Forget();
            }
        }
    }
}
