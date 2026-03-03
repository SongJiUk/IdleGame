using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuff
{
    //���� �ߺ� üũ / �������� ������ ��ü���� Ÿ�� ��ȯ
    System.Type GetBuffType();

    //������ ��ü�� ����ɶ�, ���� ���� ����ȿ�� �߻�
    void Apply(CreatureController _target);
    //���� ����� ����
    void Remove(CreatureController _target);

    //BuffController�� Tick()���� ���ư� �̸��� update
    void Update(float _deltaTime);
    bool isExpired();
}
