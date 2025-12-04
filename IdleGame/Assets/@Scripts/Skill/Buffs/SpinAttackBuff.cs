using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO : 이걸 합쳐버릴까?
public class SpinAttackBuff : BuffBase
{
    CreatureController caster;
    //TODO : 이런것들 다 하드코딩 지우기.
    const float TICK_INTERVAL = 2f / 3f;
    float damageMultiplier;
    float timeSinceLastTick;

    public SpinAttackBuff(CreatureController _caster, float _duration) : base(_duration) { this.caster = _caster; }

    public override void Apply(CreatureController _target) { }

    public override void Remove(CreatureController _target) { }


    public override void Update(float _deltaTime)
    {
        //남은 시간 감소
        base.Update(_deltaTime);

        timeSinceLastTick += _deltaTime;
        if (timeSinceLastTick >= TICK_INTERVAL)
        {
            DealAreaDamage();
            timeSinceLastTick = 0f;
        }
    }

    void DealAreaDamage()
    {
        List<CreatureController> hitEnemies = Utils.FindEnemyInSphereArea(caster, 2f);
        float tickDamage = (float)caster.Damage * damageMultiplier;
        foreach (CreatureController enemy in hitEnemies)
        {
            enemy.GetDamage(tickDamage, caster);
        }
    }



}
