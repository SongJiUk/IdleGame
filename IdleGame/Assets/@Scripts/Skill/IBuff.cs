using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuff
{
    //버프 중복 체크 / 관리위해 버프의 구체적인 타입 반환
    System.Type GetBuffType();

    //버프가 객체에 적용될때, 스탯 변경 실제효과 발생
    void Apply(CreatureController _target);
    //버프 만료시 제거
    void Remove(CreatureController _target);

    //BuffController의 Tick()에서 돌아감 이름만 update
    void Update(float _deltaTime);
    //버프 만료 확인
    bool isExpired();
}
