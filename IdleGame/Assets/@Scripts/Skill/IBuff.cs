using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuff
{
    void Apply(CreatureController _target);
    void Remove(CreatureController _target);
    void Update(float _deltaTime);
    //버프 만료
    bool isExpired();
}
