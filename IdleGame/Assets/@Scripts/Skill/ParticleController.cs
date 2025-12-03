using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{

    //파티클시스템 callback함수 이용한거.
    private void OnParticleSystemStopped()
    {
        Managers.ResourceM.Destroy(gameObject);
    }

}
