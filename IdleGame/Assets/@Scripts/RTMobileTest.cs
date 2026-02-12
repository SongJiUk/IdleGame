using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RTMobileTest : MonoBehaviour
{
    public RawImage raw;

    void Start()
    {
        var rt = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
        rt.Create();

        var cam = GetComponent<Camera>();
        cam.targetTexture = rt;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.red;

        raw.texture = rt;
    }
}
