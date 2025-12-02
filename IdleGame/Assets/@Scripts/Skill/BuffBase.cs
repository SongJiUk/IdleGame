using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffBase : IBuff
{

    //TODO: 안쓰면 제거
    protected CreatureController target;
    protected float duration;
    protected float leftTime;


    //IBuff 인터페이스 구현 : 버프의 구체적인 타입 반환
    public System.Type GetBuffType()
    {
        return this.GetType();
    }

    //생성자 : 버프 지속시간 설정, 남은시간 초기화
    public BuffBase(float _duration)
    {
        this.duration = _duration;
        this.leftTime = _duration;
    }
    public virtual void Update(float _deltaTime)
    {
        leftTime -= _deltaTime;
    }

    public bool isExpired()
    {
        return leftTime <= 0;
    }


    //적용 제거 로직
    public abstract void Apply(CreatureController _target);
    public abstract void Remove(CreatureController _target);

}
