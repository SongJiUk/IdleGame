using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum RenderType
{
    Hero,
    Gacha,
    Dungeon,
    HereStat,
    Saving
}
public class RenderManager : MonoBehaviour
{
    [Header("Render Camera")]
    public Camera heroCamera;
    public Camera gachaCamera;
    public Camera dungeonCamera;
    public Camera heroStatCamera;
    public Camera savingCamera;

    [Header("Render Object")]
    public GameObject heroObject;
    public GameObject gachaObject;
    public GameObject dungeonObject;
    public GameObject heroStatObject;
    public GameObject savingObject;

    [Header("Dungeon NPC")]
    public List<DungeonNPCController> npcs;
    GameObject currentGameObject;
    Camera currentCamera;
    RenderTexture currentRT;

    public RenderCharacter renderCharacter;
    public RenderGacha renderGacha;

    private void Awake()
    {
        DisableAllObject();
    }

    private void Start()
    {
        if (Managers.Instance != null)
        {
            Managers.Instance.SetRenderManager(this);
        }
        else
        {
            Debug.LogError("Managers 인스턴스가 없음");
        }
    }
    public void DisableAllObject()
    {
        DisableAllCamera();

        if (heroObject != null) heroObject.SetActive(false);
        if (gachaObject != null) gachaObject.SetActive(false);
        if (dungeonObject != null) dungeonObject.SetActive(false);
        if (heroStatObject != null) heroStatObject.SetActive(false);
        if (savingObject != null) savingObject.SetActive(false);

        
    }

    public void DisableAllCamera()
    {
        if (heroCamera != null) heroCamera.gameObject.SetActive(false);
        if (gachaCamera != null) gachaCamera.gameObject.SetActive(false);
        if (dungeonCamera != null) dungeonCamera.gameObject.SetActive(false);
        if (heroStatCamera != null) heroStatCamera.gameObject.SetActive(false);
        if (savingCamera != null) savingCamera.gameObject.SetActive(false);
    }


    Camera GetCamera(RenderType _type)
    {
        switch(_type)
        {
            case RenderType.Hero: return heroCamera;
            case RenderType.Gacha: return gachaCamera;
            case RenderType.Dungeon: return dungeonCamera;
            case RenderType.HereStat: return heroStatCamera;
            case RenderType.Saving: return savingCamera;
        }
        return null;
    }
    GameObject GetObject(RenderType _type)
    {
        switch (_type)
        {
            case RenderType.Hero: return heroObject;
            case RenderType.Gacha: return gachaObject;
            case RenderType.Dungeon: return dungeonObject;
            case RenderType.HereStat: return heroStatObject;
            case RenderType.Saving: return savingObject;
        }
        return null;
    }

    public void DungeonNPC()
    {
        for(int i =0; i<npcs.Count; i++)
        {
            npcs[i].SetInfo();
        }
    }

    public void Show(RenderType _type, RawImage _targetRawImage)
    {
        Hide();
        currentGameObject = GetObject(_type);
        currentGameObject.SetActive(true);

        var cam = GetCamera(_type);
        if(cam == null)
        {
            Debug.LogError($"RenderManager : Camera for {_type} is null");
            return;
        }

        cam.gameObject.SetActive(true);
        cam.enabled = true;

        currentRT = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
        currentRT.Create();

        cam.clearFlags = CameraClearFlags.SolidColor;
        //cam.backgroundColor = Color.black;
        cam.backgroundColor = new Color(0, 0, 0, 0);
        cam.targetTexture = currentRT;

        _targetRawImage.texture = currentRT;
        currentCamera = cam;
    }

    public void Hide()
    {
        if(currentCamera != null)
        {
            currentCamera.targetTexture = null;
            currentCamera.enabled = false;
            currentCamera.gameObject.SetActive(false);
            currentCamera = null;
        }

        if(currentRT != null)
        {
            currentRT.Release();
            Destroy(currentRT);
            currentRT = null;
        }

        if(currentGameObject  != null)
        {
            currentGameObject.SetActive(false);
        }
    }
}
