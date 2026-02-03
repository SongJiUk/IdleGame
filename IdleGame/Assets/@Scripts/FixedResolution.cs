using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//고정 해상도
public class FixedResolution : MonoBehaviour
{
    public int targetWidth = 1440;
    public int targetHeight = 2560;

    void Start()
    {
        ApplyFixedResoultion();
    }

    private void ApplyFixedResoultion()
    {
        //NOTE : 상대적으로 위 아래가 양옆보다 값이 크기때문에, 양 옆을맞추는게 자연스러움
        float targetAspect = (float)targetWidth / (float)targetHeight;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        //고정 해상도는 ui의 overlay를 camera로 해줘야한다.
        Camera cam = Camera.main;

        if (scaleHeight <= 1.0f) //검은 여백이 위아래로 생긴다
        {
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else //좌우로 검은 여백
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
        Screen.SetResolution(targetWidth, targetHeight, true);
    }
}
