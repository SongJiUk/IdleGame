using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour,ITickable
{
    float dist = 4.0f;
    [Range(0.0f, 10.0f)]
    [SerializeField] float standard_Distance = 4f;

    Camera cam;
    private void OnDisable() => Managers.UpdateM.UnRegister(this);

    private void Start()
    {
        Managers.UpdateM.Register(this);
        cam = GetComponent<Camera>();
    }
    public void Tick(float _deltaTime)
    {
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, Distance(), _deltaTime * 2f);
    }

    float Distance()
    {
        var players = Managers.ObjectM.pcSet;
        float maxDist = dist;
        foreach(var player in players)
        {
            float targetDist = Vector3.Distance(Vector3.zero, player.transform.position) + standard_Distance;

            if(targetDist > maxDist)
            {
                maxDist = targetDist;
            }
        }   

        return maxDist;
    }
}
