using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{

    public override bool Init()
    {
        if (!base.Init()) return false;

        return true;
    }

    public Vector2 PivotPoint(Vector2 _pos)
    {
        float xPos = _pos.x > Screen.width / 2 ? 1.0f : 0.0f;
        float yPos = _pos.y > Screen.height / 2 ? 1.0f : 0.0f;

        return new Vector2(xPos, yPos);
    }
}
