using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager 
{
    public int Level;
    public double Exp;
    public double BaseAttack = 10;
    public double BaseHP = 50;

    public void ExpUp()
    {
        
    }

    public float ExpPercent()
    {
        //Managers.GameM.gameData.exp;
        return 1;

    }

    public double NextAttack()
    {
        return BaseAttack * Mathf.Pow(1.08f, Level - 1);
    }

    public double NextHp()
    {
        return BaseHP * Mathf.Pow(1.10f, Level - 1);
    }

    public float NextExp()
    {
        //float exp = 
        //float myexp;
        return 1;
    }
}
