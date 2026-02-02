using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public void ExpUp()
    {
        Managers.GameM.Exp += Utils.Datas.levelData.Exp();
        Managers.GameM.UpgradeCount++;
        Managers.GameM.gameData.damage += Utils.Datas.levelData.Damage();
        Managers.GameM.gameData.hp += Utils.Datas.levelData.HP();

        if (Managers.GameM.Exp >= Utils.Datas.levelData.MaxExp())
        {
            LevelUp();
        }

        for (int i = 0; i < Managers.CharacterM.players.Length; i++)
        {
            if (Managers.CharacterM.players[i] == null) continue;

            Managers.CharacterM.players[i].InitStat();
        }
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

    public double GetAttack(Define.Grade _grade, CharacterHolder _holder)
    {
        var damage = Utils.Datas.levelData.Damage() * ((int)_grade + 1);
        float level = (float)_holder.holder.Level * 10 / (float)100;
        var realDamage = damage + damage * level;
        realDamage += realDamage * (Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.Damage) / 100f);
        return realDamage;
    }

    public double GetHP(Define.Grade _grade, CharacterHolder _holder)
    {
        var hp = Utils.Datas.levelData.HP() * ((int)_grade + 1);
        float level = (float)_holder.holder.Level * 10 / (float)100;
        var realHp = hp + hp * level;
        realHp += realHp * (Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.HP) / 100f);
        return realHp;
    }
    public void LevelUp()
    {
        Managers.GameM.Level++;
        Managers.GameM.Exp = 0;

        Managers.GameM.GetMission(Define.MissionTarget.LevelUp).Progress++;
    }

    public double MainAttack()
    {
        double attack = GetAttack(Managers.GameM.mPlayer.DATA.CharacterGrade, Managers.GameM.gameData.Characters_Data[Managers.GameM.mPlayer.DATA.Name]);
        int value = 1;
        for (int i = 0; i < Managers.CharacterM.Characters.Length; i++)
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
        return 0.0f + Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.Money);
    }

    public float ItemDrop()
    {
        return 0.0f + Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.Item); ;
    }

    public float AttackSpeed()
    {
        return 1.0f + Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.AttackSpeed);
    }

    public float CriticalChance()
    {
        return 20.0f + Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.CriticalP); ;
    }

    public float CriticalDamage()
    {
        return 140.0f + Managers.GameM.gameData.GetValueSmelt(Define.Status_Holder.CriticalD); ;
    }

    public double AverageCombatPower()
    {
        return MainAttack() + MainHP();
    }

}
