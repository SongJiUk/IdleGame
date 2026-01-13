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

    public double MainAttack()
    {
        double attack = GetAttack(Managers.GameM.mPlayer.DATA.CharacterGrade, Managers.GameM.gameData.Characters_Data[Managers.GameM.mPlayer.DATA.Name]);
        int value = 1;
        for(int i =0; i<Managers.CharacterM.Characters.Length; i++)
        {
            if (Managers.CharacterM.Characters[i] != null)
            {
                var data = Managers.CharacterM.Characters[i].data;
                attack += GetAttack(data.CharacterGrade, Managers.GameM.gameData.Characters_Data[data.Name]);
                value++;
            }
        }

        return attack / value;
    }


    public double MainHP()
    {
        double hp = GetHP(Managers.GameM.mPlayer.DATA.CharacterGrade, Managers.GameM.gameData.Characters_Data[Managers.GameM.mPlayer.DATA.Name]);
        int value = 1;
        for (int i = 0; i < Managers.CharacterM.Characters.Length; i++)
        {
            if (Managers.CharacterM.Characters[i] != null)
            {
                var data = Managers.CharacterM.Characters[i].data;
                hp += GetHP(data.CharacterGrade, Managers.GameM.gameData.Characters_Data[data.Name]);
                value++;
            }
        }

        return hp / value;
    }

    public float GoldDrop()
    {
        return 0.0f;
    }

    public float ItemDrop()
    {
        return 0.0f;
    }

    public float AttackSpeed()
    {
        return 1.0f;
    }

    public float CriticalChance()
    {
        return 20.0f;
    }

    public float CriticalDamage()
    {
        return 140.0f;
    }

    public double AverageCombatPower()
    {
        return MainAttack() + MainHP();
    }

}
