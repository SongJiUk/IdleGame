using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : CreatureController
{

    void OnEnable() => Managers.UpdateM.Register(this);
    void OnDisable() => Managers.UpdateM.UnRegister(this);


    public override bool Init()
    {
        if (!base.Init()) return false;


        return true;
    }

    public override void SetInfo()
    {
        base.SetInfo();

    }

    public override void InitStat()
    {

    }

    public override void OnDamage()
    {

    }

    public override void OnDead()
    {

    }

    public override void Tick(float _deltaTime)
    {
        if (isDead) return;
        FindClosetTarget(Managers.ObjectM.mcSet);
        if (target.gameObject != null)
        {

        }
        else
        {
            if (target.IsDead)
                FindClosetTarget(Managers.ObjectM.mcSet);

            //TODO: 상대 찾아서 죽이기
        }
    }
}
