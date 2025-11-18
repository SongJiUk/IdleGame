using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public PlayerController mPlayer { get { return Managers.ObjectM?.mPlayer; } }
    public GameData gameData = new GameData();

    #region 재화 이벤트
    public event Action OnGoodsChanged;
    #endregion

    double gold;
    public double Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            OnGoodsChanged?.Invoke();
        }
    }
    public int level;
    public double exp;
    public int stage;



}
