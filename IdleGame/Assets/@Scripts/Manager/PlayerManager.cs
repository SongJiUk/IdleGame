using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{

    public double Damage;
    public double Hp;

    public void ExpUp()
    {
        Managers.GameM.Exp += Utils.Datas.levelData.Exp();

        if (Managers.GameM.Exp >= Utils.Datas.levelData.MaxExp())
        {
            LevelUp();
        }

        for (int i = 0; i < Managers.SpawnM.players.Count; i++) Managers.SpawnM.players[i].InitStat();
    }

    public float ExpPercent()
    {
        float exp = (float)Utils.Datas.levelData.MaxExp();
        double myExp = Managers.GameM.Exp;

        return (float)myExp / exp;
    }

    public float NextExp()
    {
        float exp = (float)Utils.Datas.levelData.MaxExp();
        float myExp = (float)Utils.Datas.levelData.Exp();

        return (myExp / exp) * 100.0f;
    }

    public double GetAttack(Define.CharacterGrade _grade, CharacterHolder _holder)
    {
        var damage = Utils.Datas.levelData.Damage() * ((int)_grade + 1);
        float level = (float)_holder.holder.Level * 10 / (float)100;
        var realDamage = damage + damage * level;

        return realDamage;
    }

    public double GetHP(Define.CharacterGrade _grade, CharacterHolder _holder)
    {
        var hp = Utils.Datas.levelData.HP() * ((int)_grade + 1);
        float level = (float)_holder.holder.Level * 10 / (float)100;
        var realHp = hp + hp * level;

        return realHp;
    }
    public void LevelUp()
    {
        Managers.GameM.Level++;
        Damage += Utils.Datas.levelData.Damage();
        Hp += Utils.Datas.levelData.HP();
        Managers.GameM.Exp = 0;
    }
}
