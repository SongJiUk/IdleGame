using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class HealBuff : BuffBase
{
    float healPerTick;

    public HealBuff(float _duration, float _ratio, float _interval) : base(_duration)
    {
        this.duration = _duration;
        this.ratio = _ratio;
        this.interval = _interval;
        int tickCount = Mathf.Max(1, Mathf.RoundToInt(this.duration / interval));
        healPerTick = ratio / tickCount;
    }

    public override void Apply(CreatureController _target)
    {
        target = _target;
        timeSinceLastTick = 0f;
        Debug.Log("[HealBuff] 힐 시작");

    }

    public override void Update(float _deltaTime)
    {
        base.Update(_deltaTime);

        timeSinceLastTick += _deltaTime;
        if (timeSinceLastTick >= interval)
        {
            timeSinceLastTick = 0f;
            target.Heal(healPerTick);
            Debug.Log($"[HealBuff] 틱 회복 : {healPerTick}");
        }
    }

    public override void Remove(CreatureController _target) { Debug.Log("[HealBuff] 종료"); }
    public override Define.BuffEffectType GetBuffTypes() => Define.BuffEffectType.None;
}
