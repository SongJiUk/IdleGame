using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public PlayerController mPlayer { get { return Managers.ObjectM?.mPlayer; } }
    public GameData gameData = new GameData();
}
