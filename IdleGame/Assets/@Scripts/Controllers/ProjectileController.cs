using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading; // OperationCanceledException을 위해 필요

public class ProjectileController : BaseController
{
    Dictionary<string, GameObject> Projectiles = new Dictionary<string, GameObject>();
    Dictionary<string, ParticleSystem> Muzzles = new Dictionary<string, ParticleSystem>();
    string characterName;
    MonsterController target;
    Vector3 targetPos;
    bool isHit = false;
    double damage;
    

    private void Awake()
    {
        Transform projectiles = transform.GetChild(0);
        Transform muzzles = transform.GetChild(1);

        for(int i = 0; i<projectiles.childCount; i++)
            Projectiles.Add(projectiles.GetChild(i).name, projectiles.GetChild(i).gameObject);
        

        for(int i =0; i<muzzles.childCount; i++)
            Muzzles.Add(muzzles.GetChild(i).name , muzzles.GetChild(i).GetComponent< ParticleSystem>());
    }

    public void Init(MonsterController _mc, double _dmg, string _characterName)
    {
        Managers.UpdateM.Register(this);

        target = _mc;
        transform.LookAt(target.transform);
        isHit = false;
        targetPos = target.transform.position;

        damage = _dmg;
        characterName = _characterName;
        Projectiles[characterName].SetActive(true);
    }

    public override void Tick(float _deltaTime)
    {
        if (isHit) return;

        //TODO : 파티클 높이 문제
        targetPos.y = 0.5f;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, _deltaTime* 5f);

        if(Vector3.Distance(transform.position, targetPos) <= 0.1f)
        {
            if(target != null)
            {
                isHit = true;
                target.GetDamage(damage);


                //TODO : 닿았을때
                Projectiles[characterName].SetActive(false);
                Muzzles[characterName].Play();

                ReturnObject(Muzzles[characterName].duration).Forget();
            }
        }
    }


    public override async UniTask ReturnObject(float _time)
    {
        //NOTE: 객체가 파괴되거나, 수동취소시 사용하는 코드라는데
        CancellationToken token = this.GetCancellationTokenOnDestroy();
        try
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(_time), cancellationToken : token);
            Managers.UpdateM.UnRegister(this);
            Managers.ObjectM.DeSpawn(this);

        }
        catch(System.OperationCanceledException){}
        catch(System.Exception e)
        {
            Managers.UpdateM.UnRegister(this);
            Debug.LogError($"Projectile 반환중 에러 발생 {e.Message}");
        }
    }
}
