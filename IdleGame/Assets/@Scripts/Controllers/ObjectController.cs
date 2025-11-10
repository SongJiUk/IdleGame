using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ObjectController : BaseController
{
    //TODO : Smoke가 비활성화 되는 이유가 뭘지 생각해보기ㄴ
    ParticleSystem particle;
    public override bool Init()
    {

        if (!base.Init()) return false;

        if(particle == null)
            particle = GetComponent<ParticleSystem>();

        Play(particle.duration);
        return true;
    }
    
    void Play(float _time)
    {
        if (particle != null)
            particle.Play();

        ReturnObject(_time).Forget();

        Managers.ObjectM.DeSpawn(this);
    }
  
}
