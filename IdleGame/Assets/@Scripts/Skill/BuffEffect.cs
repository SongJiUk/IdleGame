using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��ų ȿ���� �ϳ�, Ÿ�ٿ��� ���� �����ϴ� ����
public class BuffEffect : ISkillEffect
{
    //IBuff �ν��Ͻ� �����ϴ� ���丮�Լ�(���÷��� ��� ���)
    public delegate IBuff BuffFactory(float _duration, float _ratio, float _interval = 0, CreatureController _owner = null);
    readonly BuffFactory buffFactory;

    private float buffDurtaion;
    float buffRatio;
    float buffInterval;
    CreatureController buffOwner;

    //���� �ν��Ͻ��� ������ ���丮 �Լ�, ���ӽð�
    public BuffEffect(BuffFactory _factory, float _duration, float _ratio, float _interval = 0, CreatureController _owner = null)
    {
        buffFactory = _factory;
        buffDurtaion = _duration;
        buffRatio = _ratio;
        buffInterval = _interval;
        buffOwner = _owner;

    }

    public void Execute(CreatureController _caster, CreatureController _target, float _delayTime = 0f)
    {
        //���� �ν��Ͻ� ���� : ���丮 �Լ��� ȣ���Ͽ� ���ο� Ibuff ��ü ����
        IBuff newBuff = buffFactory(buffDurtaion, buffRatio, buffInterval, buffOwner);
        //Ÿ�ٿ� ���� �߰�, Ÿ���� addfBuffȣ��
        _target.buffController.AddBuff(newBuff);


        //TDOO: �??���?(?��?��방법)
        //effects.Add(new BuffEffect(typeof(AttackBuff), 15f));
    }
}
