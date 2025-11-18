using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager 
{
    
    public double Damage;
    public double Hp;

    public void ExpUp()
    {
        Managers.GameM.exp += Utils.Datas.levelData.Exp();
        Damage += Utils.Datas.levelData.Damage((float)Managers.GameM.mPlayer.BaseDamage);
        Hp += Utils.Datas.levelData.HP((float)Managers.GameM.mPlayer.BaseHp);

        if(Managers.GameM.exp >= Utils.Datas.levelData.MaxExp())
        {
            Managers.GameM.level++;
            Managers.GameM.exp = 0;
        }

        for (int i = 0; i < Managers.SpawnM.players.Count; i++) Managers.SpawnM.players[i].InitStat();
    }

    public float ExpPercent()
    {
        float exp = (float)Utils.Datas.levelData.MaxExp();
        double myExp = Managers.GameM.exp;

        return (float)myExp / exp;
    }

    public float NextExp()
    {
        float exp = (float)Utils.Datas.levelData.MaxExp();
        float myExp = (float)Utils.Datas.levelData.Exp();

        return (myExp / exp) * 100.0f;
    }
  

}
