using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class ProjectileController : BaseController
{

    protected string characterName;
    protected CreatureController target;
    protected Vector3 targetPos;
    protected bool isHit = false;
    protected double damage;


    private void Awake()
    {

    }


    public override bool Init()
    {
        if (!base.Init()) return false;

        return true;
    }
    //TODO: 
    public virtual void AttackInit(CreatureController _cc, double _dmg, CreatureController _owner, bool _isSkillProjectile = false)
    {
        Managers.UpdateM.Register(this);
        target = _cc;

        isHit = false;
        damage = _dmg;
        characterName = _owner.name;
    }

    public override void Tick(float _deltaTime)
    {

    }


    public virtual async UniTask ReturnObject(float _time)
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();
        try
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(_time), cancellationToken: token);
            Managers.UpdateM.UnRegister(this);
            Managers.ObjectM.DeSpawn(this);

        }
        catch (System.OperationCanceledException) { }
        catch (System.Exception e)
        {
            Managers.UpdateM.UnRegister(this);
            Debug.LogError($"Projectile Exception :  {e.Message}");
        }
    }
}
