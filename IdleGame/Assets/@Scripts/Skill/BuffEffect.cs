using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//스킬 효과중 하나, 타겟에게 버프 적용하는 역할
public class BuffEffect : ISkillEffect
{
    //IBuff 인스턴스 생성하는 팩토리함수(리플렉션 대신 사용)
    public delegate IBuff BuffFactory(float _duration);
    readonly BuffFactory buffFactory;

    private float buffDurtaion;

    //버프 인스턴스를 생성할 팩토리 함수, 지속시간
    public BuffEffect(BuffFactory _factory, float _duration)
    {
        buffFactory = _factory;
        buffDurtaion = _duration;
    }

    public void Excute(CreatureController _caster, CreatureController _target)
    {
        //버프 인스턴스 생성 : 팩토리 함수를 호출하여 새로운 Ibuff 객체 생성
        IBuff newBuff = buffFactory(buffDurtaion);
        //타겟에 버프 추가, 타겟의 addfBuff호출
        _target.buffController.AddBuff(newBuff);


        //TDOO: 吏??슱嫄?(?궗?슜諛⑸쾿)
        //effects.Add(new BuffEffect(typeof(AttackBuff), 15f));
    }
}
