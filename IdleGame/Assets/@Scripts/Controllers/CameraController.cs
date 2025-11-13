using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Unity.VisualScripting;

public class CameraController : MonoBehaviour, ITickable
{

    private CancellationTokenSource cts = new CancellationTokenSource();
    public CancellationToken cancellationToken => cts.Token;
    float dist = 4.0f;
    [Range(0.0f, 10.0f)]
    [SerializeField] float standard_Distance = 4f;

    [Range(0.0f, 10.0f)][SerializeField] float duration;
    public float Duration { get { return duration; } }
    [Range(0.0f, 10.0f)][SerializeField] float power;
    public float Power { get { return power; } }


    Vector3 originPos;
    public Vector3 OriginPos { get { return originPos; } }
    public bool isCameraShake = false;
    Camera cam;
    private void OnDisable() => Managers.UpdateM.UnRegister(this);
    void OnDestroy()
    {
        cts.Cancel();
        cts.Dispose();
    }
    private void Start()
    {
        Managers.UpdateM.Register(this);
        cam = GetComponent<Camera>();

        Managers.CameraM.SetController(this);

        originPos = transform.localPosition;
    }
    public void Tick(float _deltaTime)
    {
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, Distance(), _deltaTime * 2f);
    }

    float Distance()
    {
        var players = Managers.ObjectM.pcList;
        float maxDist = dist;
        foreach (var player in players)
        {
            float targetDist = Vector3.Distance(Vector3.zero, player.transform.position) + standard_Distance;

            if (targetDist > maxDist)
            {
                maxDist = targetDist;
            }
        }

        return maxDist;
    }


}
