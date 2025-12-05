using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO : 이걸 합쳐버릴까?
public class SpinAttackBuff : BuffBase
{
    //TODO : 이런것들 다 하드코딩 지우기.
    CreatureController owner;

    public SpinAttackBuff(float _duration, float _ratio, float _radius, float _interval, CreatureController _owner = null) : base(_duration)
    {
        ratio = _ratio;
        radius = _radius;
        interval = _interval;
        owner = _owner;
    }

    public override void Apply(CreatureController _target)
    {
        target = _target;
    }

    public override void Remove(CreatureController _target)
    {
        target = null;
    }


    public override void Update(float _deltaTime)
    {
        //남은 시간 감소
        base.Update(_deltaTime);

        timeSinceLastTick += _deltaTime;
        if (timeSinceLastTick >= interval)
        {
            DealAreaDamage();
            timeSinceLastTick = 0f;
        }
    }

    void DealAreaDamage()
    {

        //TODO : 여기서 target은 버프를 받은 사람임(맞은 사람)
        float tickDamage = (float)owner.Damage * ratio;

        target.GetDamage(tickDamage, target);
    }



}
