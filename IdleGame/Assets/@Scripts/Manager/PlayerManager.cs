using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager 
{
    
    public double Damage;
    public double Hp;

    public void ExpUp()
    {
        Managers.GameM.Exp += Utils.Datas.levelData.Exp(Managers.GameM.Level);
        Damage += Utils.Datas.levelData.Damage((float)Managers.GameM.mPlayer.BaseDamage, Managers.GameM.Level);
        Hp += Utils.Datas.levelData.HP((float)Managers.GameM.mPlayer.BaseHp, Managers.GameM.Level);

        if(Managers.GameM.Exp >= Utils.Datas.levelData.MaxExp(Managers.GameM.Level))
        {
            Managers.GameM.Level++;
            Managers.GameM.Exp = 0;
        }

        for (int i = 0; i < Managers.SpawnM.players.Count; i++) Managers.SpawnM.players[i].InitStat();
    }

    public float ExpPercent()
    {
        float exp = (float)Utils.Datas.levelData.MaxExp(Managers.GameM.Level);
        double myExp = Managers.GameM.Exp;

        return (float)myExp / exp;
    }

    public float NextExp()
    {
        float exp = (float)Utils.Datas.levelData.MaxExp( Managers.GameM.Level);
        float myExp = (float)Utils.Datas.levelData.Exp( Managers.GameM.Level);

        return (myExp / exp) * 100.0f;
    }
  

}
